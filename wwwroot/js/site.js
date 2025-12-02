// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener('DOMContentLoaded', () => {
  const container = document.getElementById('apps-row');
  let dragged = null;

  // dragstart / dragend au container (délégué)
  container.addEventListener('dragstart', (e) => {
    const item = e.target.closest('.app-item');
    if (!item) return;
    dragged = item;
    item.classList.add('dragging');
    // nécessaire pour Firefox
    try { e.dataTransfer.setData('text/plain', item.dataset.id || ''); } catch (err) {}
    e.dataTransfer.effectAllowed = 'move';
  });

  container.addEventListener('dragend', () => {
    if (dragged) dragged.classList.remove('dragging');
    dragged = null;
  });

  container.addEventListener('dragover', (e) => {
    e.preventDefault();
    const after = getDragAfterElement(container, e.clientX);
    if (!dragged) return;
    if (after == null) container.appendChild(dragged);
    else container.insertBefore(dragged, after);
  });

  container.addEventListener('drop', (e) => {
    e.preventDefault();
    // Optionnel : sauvegarder l'ordre
    const order = Array.from(container.querySelectorAll('.app-item'))
                       .map(el => el.dataset.id);
    console.log('Nouvel ordre :', order);
  });

  function getDragAfterElement(container, x) {
    const draggableElements = [...container.querySelectorAll('.app-item:not(.dragging)')];
    let closest = null;
    let closestOffset = Number.NEGATIVE_INFINITY;
    for (const child of draggableElements) {
      const box = child.getBoundingClientRect();
      const offset = x - (box.left + box.width / 2);
      // on veut le premier enfant dont centre est à droite du curseur (offset < 0)
      if (offset < 0 && offset > closestOffset) {
        closestOffset = offset;
        closest = child;
      }
    }
    return closest;
  }
});