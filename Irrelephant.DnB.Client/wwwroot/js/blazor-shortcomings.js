function attachDragEvent(element) {
    element.addEventListener("ondragstart", (dragEvent) => {
        dragEvent.dataTransfer.setDragImage(null);
    });
}