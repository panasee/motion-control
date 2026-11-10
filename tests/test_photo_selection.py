"""Original-pixel selection semantics and polygon conversion."""
import unittest

import cv2
import numpy as np

from photo_selection import SelectionModel


class SelectionTests(unittest.TestCase):
    def test_wand_fixed_seed_range_contiguity_and_source_unchanged(self):
        rgb = np.full((20, 40, 3), 200, np.uint8)
        rgb[2:8, 2:8] = 50
        rgb[2:8, 20:26] = 50
        rgb[2:8, 8] = 60
        rgb[2:8, 9] = 70
        model = SelectionModel(rgb)
        local = model.wand(3, 3, 10)
        self.assertEqual(np.count_nonzero(local), 42)
        self.assertEqual(local[3, 9], 0)  # No gradual color leakage.
        self.assertEqual(local[3, 20], 0)
        self.assertEqual(np.count_nonzero(model.wand(3, 3, 10, False)), 78)
        np.testing.assert_array_equal(model.rgb, rgb)
        self.assertEqual(np.count_nonzero(model.mask), 0)

    def test_boolean_operations_undo_redo_and_new_branch(self):
        model = SelectionModel(np.zeros((5, 7, 3), np.uint8))
        a = np.zeros((5, 7), np.uint8)
        a[1:4, 1:4] = 255
        b = np.zeros_like(a)
        b[2:5, 2:5] = 255
        for operation, expected in (("new", 9), ("add", 14), ("subtract", 5), ("intersect", 4)):
            model.commit(a)
            model.apply(b, operation)
            self.assertEqual(np.count_nonzero(model.mask), expected)
            model.undo()
            np.testing.assert_array_equal(model.mask, a)
            model.redo()
            self.assertEqual(np.count_nonzero(model.mask), expected)
        model.undo()
        model.apply(b, "add")
        self.assertFalse(model.redo_stack)

    def test_holes_are_explicit_and_fill_is_undoable(self):
        model = SelectionModel(np.zeros((40, 40, 3), np.uint8))
        mask = model.mask.copy()
        mask[0:30, 0:30] = 255  # Also test selection touching photo boundary.
        mask[10:20, 10:20] = 0
        model.commit(mask)
        with self.assertRaisesRegex(ValueError, "孔洞"):
            model.polygons()
        model.fill_holes()
        self.assertEqual(np.count_nonzero(model.mask), 900)
        self.assertEqual(len(model.polygons()), 1)
        model.undo()
        np.testing.assert_array_equal(model.mask, mask)

    def test_original_resolution_small_features_and_multiple_regions(self):
        model = SelectionModel(np.zeros((1800, 2400, 3), np.uint8))
        mask = model.mask.copy()
        points = np.array([[100, 100], [300, 100], [300, 200], [203, 200],
                           [203, 197], [197, 197], [197, 200], [100, 200]], np.int32)
        cv2.fillPoly(mask, [points], 255)
        mask[1500:1510, 2000:2010] = 255
        model.commit(mask)
        exact = model.polygons(0)
        coarse = model.polygons(5)
        self.assertEqual(len(exact), 2)
        self.assertGreater(len(exact[0]), len(coarse[0]))
        np.testing.assert_array_equal(np.max(exact[1], axis=0), [2009, 1509])
        restored = np.zeros_like(mask)
        cv2.fillPoly(restored, [np.array(p, np.int32) for p in exact], 255)
        np.testing.assert_array_equal(restored, mask)

    def test_empty_degenerate_and_over_simplified_selections_fail(self):
        model = SelectionModel(np.zeros((20, 20, 3), np.uint8))
        with self.assertRaisesRegex(ValueError, "建立选区"):
            model.polygons()
        mask = model.mask.copy()
        mask[2, 2] = 255
        model.commit(mask)
        with self.assertRaisesRegex(ValueError, "孤立点"):
            model.polygons()
        mask[2:5, 2:5] = 255
        model.commit(mask)
        with self.assertRaisesRegex(ValueError, "误差过大"):
            model.polygons(20)

    def test_history_is_bounded_and_noop_keeps_redo(self):
        model = SelectionModel(np.zeros((10, 10, 3), np.uint8))
        for i in range(30):
            mask = model.mask.copy()
            mask.flat[i] = 255
            model.commit(mask)
        self.assertEqual(len(model.undo_stack), 20)
        model.undo()
        self.assertFalse(model.commit(model.mask))
        self.assertTrue(model.redo())
