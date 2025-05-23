﻿/* Global scripting for the app */

// Ensure the DOM is fully loaded before executing any of this
document.addEventListener('DOMContentLoaded', function () {
    let currentModal = null;

    // Attach a single click listener to the body – useful for dynamic content like modal buttons
    document.body.addEventListener('click', function (e) {
        const editBtn = e.target.closest('[data-edit-product-id]');
        if (!editBtn) return;

        e.preventDefault(); // Just in case it's inside a <form> or <a>

        const productId = editBtn.getAttribute('data-edit-product-id');
        console.log('Trying to edit product with ID:', productId);

        // Clean up any existing modal
        if (currentModal) {
            const bsModalInstance = bootstrap.Modal.getInstance(currentModal);
            if (bsModalInstance) {
                bsModalInstance.dispose();
            }
            currentModal.remove();
            currentModal = null;
        }

        // Go fetch the modal HTML via AJAX (server returns partial view)
        fetch(`/Farmer/GetEditProductModal/${productId}`, {
            method: 'GET',
            headers: {
                'Accept': 'text/html',
                'X-Requested-With': 'XMLHttpRequest'
            }
        })
            .then(response => {
                if (!response.ok) throw new Error(`Server returned ${response.status}`);
                return response.text();
            })
            .then(html => {
                const tempDiv = document.createElement('div');
                tempDiv.innerHTML = html;

                const modalEl = tempDiv.querySelector('.modal');
                if (!modalEl) throw new Error('Modal element not found in server response');

                // Add modal to DOM and trigger Bootstrap
                document.body.appendChild(modalEl);
                currentModal = modalEl;

                const bsModal = new bootstrap.Modal(modalEl, {
                    backdrop: true,
                    keyboard: true,
                    focus: true
                });

                // Show modal
                bsModal.show();

                // Setup form logic
                setupEditFormSubmit(modalEl, bsModal);

                // Allow dragging the modal
                makeModalDraggable(modalEl);

                // Handle modal cleanup after it's hidden
                modalEl.addEventListener('hidden.bs.modal', function handler() {
                    bsModal.dispose();
                    modalEl.remove();
                    currentModal = null;
                    modalEl.removeEventListener('hidden.bs.modal', handler);
                }, { once: true });
            })
            .catch(error => {
                console.error('Failed to load edit modal:', error);
                alert(`Something went wrong while loading the edit form: ${error.message}`);
            });
    });

    // Submits the edit form via AJAX inside the modal
    function setupEditFormSubmit(modalEl, bsModal) {
        const form = modalEl.querySelector('#editProductForm');
        if (!form) return console.error('Edit form not found inside modal');

        form.addEventListener('submit', function (e) {
            e.preventDefault();

            const formData = new FormData(form);

            fetch(form.action, {
                method: 'POST',
                body: formData,
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            })
                .then(res => {
                    if (!res.ok) throw new Error('Could not save changes');
                    return res.text();
                })
                .then(() => {
                    bsModal.hide();
                    // Show success toast – nice and subtle
                    showToast('✅ Product updated successfully!');

                    // Refresh view (could also refresh partial if preferred)
                    setTimeout(() => window.location.reload(), 800);
                })
                .catch(error => {
                    console.error('Error saving product:', error);
                    alert('Update failed. Try again or contact support.');
                });
        });
    }

    // Displays a toast with a custom message
    function showToast(message) {
        const toastContainer = document.querySelector('.toast-container');
        const toastEl = document.getElementById('productToast');
        const toastBody = toastEl?.querySelector('.toast-body');

        if (toastBody) toastBody.textContent = message;

        const toast = new bootstrap.Toast(toastEl);
        toast.show();
    }

    // Enables dragging a Bootstrap modal by its header
    function makeModalDraggable(modal) {
        const header = modal.querySelector('.modal-header');
        const dialog = modal.querySelector('.modal-dialog');
        if (!header || !dialog) return;

        let isDragging = false;
        let offsetX = 0, offsetY = 0;
        let hasBeenMoved = false;

        // Store initial position
        const initialPosition = {
            top: dialog.getBoundingClientRect().top,
            left: dialog.getBoundingClientRect().left
        };

        header.style.cursor = 'move';

        header.addEventListener('mousedown', function (e) {
            isDragging = true;
            const rect = dialog.getBoundingClientRect();
            offsetX = e.clientX - rect.left;
            offsetY = e.clientY - rect.top;
            dialog.style.position = 'fixed';
            dialog.style.zIndex = '1055'; // Keep on top
        });

        document.addEventListener('mousemove', function (e) {
            if (!isDragging) return;
            hasBeenMoved = true;
            dialog.style.top = `${e.clientY - offsetY}px`;
            dialog.style.left = `${e.clientX - offsetX}px`;
            dialog.style.margin = '0';
        });

        document.addEventListener('mouseup', function () {
            isDragging = false;
        });

        // Reset position if modal hasn't been moved
        modal.addEventListener('hidden.bs.modal', function () {
            if (!hasBeenMoved) {
                dialog.style.position = '';
                dialog.style.top = '';
                dialog.style.left = '';
                dialog.style.margin = '';
            }
        });
    }
});