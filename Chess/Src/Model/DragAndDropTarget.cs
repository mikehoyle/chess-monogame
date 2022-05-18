#nullable enable
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Chess.Model {

  // For use by drag-and-droppables
  public struct DragAndDropState {
    public bool IsSelected;
    public Point Position;
  }

  public interface IDragAndDroppable {
    DragAndDropState DragAndDropState { get; set; }
    void OnMouseDrop(Point point);
  }
  
  public class DragAndDropTarget {
    private bool _currentlyDragging;
    private IDragAndDroppable? _target;
    
    public DragAndDropTarget() {
      _currentlyDragging =false;
    }

    // TODO handle mousing out of board/window
    public void Update(MouseState mouseState, IDragAndDroppable? hoveredTarget) {
      if (mouseState.LeftButton == ButtonState.Pressed) {
        if (hoveredTarget != null) {
          OnMousePressedOverTarget(hoveredTarget);
        }
        UpdateMousePosition(mouseState.Position);
      } else {
        OnMouseUnpressed(mouseState.Position);
      }

      if (mouseState.RightButton == ButtonState.Pressed) {
        CancelDragging();
      }
    }
    
    private void OnMousePressedOverTarget(IDragAndDroppable target) {
      if (!_currentlyDragging) {
        _target = target;
        _currentlyDragging = true;
      }
    }

    private void UpdateMousePosition(Point mousePosition) {
      if (_target != null) {
        _target.DragAndDropState = new DragAndDropState {
            IsSelected = true,
            Position = mousePosition,
        };  
      }
    }

    private void OnMouseUnpressed(Point mousePosition) {
      if (_currentlyDragging) {
        _target?.OnMouseDrop(mousePosition);
        CancelDragging();
      }
    }

    private void CancelDragging() {
      if (_target != null) {
        _target.DragAndDropState = new DragAndDropState {
            IsSelected = false,
            Position = Point.Zero,
        };
      }
      _currentlyDragging = false;
      _target = null;
    }
  }
}