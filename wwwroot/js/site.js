// This waits for the entire DOM to be ready before running anything
document.addEventListener('DOMContentLoaded', function () {
    document.body.addEventListener('click', function (e) {
        const editBtn = e.target.closest('[data-edit-product-id]');
        if (!editBtn) return;

        e.preventDefault(); // Prevent any default action

        const productId = editBtn.getAttribute('data-edit-product-id');
        console.log('Attempting to edit product:', productId); // Debug log

        // Clean up any existing modals first
        const existingModals = document.querySelectorAll('.modal');
        existingModals.forEach(modal => {
            const bsModalInstance = bootstrap.Modal.getInstance(modal);
            if (bsModalInstance) {
                bsModalInstance.dispose();
            }
            modal.parentNode.removeChild(modal);
        });

        // Fetch the modal
        fetch(`/Farmer/GetEditProductModal/${productId}`, {
            method: 'GET',
            headers: {
                'Accept': 'text/html',
                'X-Requested-With': 'XMLHttpRequest'
            }
        })
            .then(response => {
                console.log('Response status:', response.status); // Debug log
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                return response.text();
            })
            .then(html => {
                console.log('HTML received, length:', html.length); // Debug log

                // Create a temporary container
                const tempDiv = document.createElement('div');
                tempDiv.innerHTML = html;

                // Get the modal element
                const modalElement = tempDiv.querySelector('.modal');
                if (!modalElement) {
                    throw new Error('Modal element not found in response');
                }

                // Append to body
                document.body.appendChild(modalElement);

                // Force browser to reflow
                modalElement.offsetHeight;

                // Initialize Bootstrap modal with explicit options
                const options = {
                    backdrop: true,
                    keyboard: true,
                    focus: true
                };

                const bsModal = new bootstrap.Modal(modalElement, options);

                // Show modal
                bsModal.show();

                console.log('Modal should be visible now'); // Debug log

                // Setup form submission
                setupEditFormSubmit(modalElement, bsModal);
            })
            .catch(error => {
                console.error('Error loading modal:', error);
                alert('Failed to load edit form: ' + error.message);
            });
    });

    function setupEditFormSubmit(modalEl, bsModal) {
        const form = modalEl.querySelector('#editProductForm');
        if (!form) {
            console.error('Form not found in modal');
            return;
        }

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
                .then(response => {
                    if (!response.ok) {
                        throw new Error('Save failed');
                    }
                    return response.text();
                })
                .then(() => {
                    bsModal.hide();

                    // Cleanup after modal is hidden
                    modalEl.addEventListener('hidden.bs.modal', function () {
                        bsModal.dispose();
                        modalEl.parentNode.removeChild(modalEl);
                    });

                    alert('Product updated successfully.');
                    window.location.reload();
                })
                .catch(error => {
                    console.error('Error saving product:', error);
                    alert('Error saving product: ' + error.message);
                });
        });
    }
});