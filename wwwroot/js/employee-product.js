document.addEventListener('DOMContentLoaded', function () {
    // Print button functionality
    const printBtn = document.getElementById('printBtn');
    if (printBtn) {
        printBtn.addEventListener('click', function () {
            const dashboardContent = document.querySelector('.dashboard-content');
            if (!dashboardContent) return;

            // Get farmer information safely
            const farmerNameElement = document.querySelector('.farmer-avatar-medium')?.nextElementSibling?.querySelector('h4');
            const locationElement = document.querySelector('.fa-map-marker-alt')?.parentElement;

            const farmerName = farmerNameElement?.textContent || 'Unknown Farmer';
            const farmerLocation = locationElement?.textContent?.trim() || 'Location not specified';

            // Add data attributes for print
            dashboardContent.setAttribute('data-farmer-name', farmerName);
            dashboardContent.setAttribute('data-farmer-location', farmerLocation);
            dashboardContent.setAttribute('data-print-date', new Date().toLocaleString());

            // Prepare table for print
            const table = document.getElementById('productsTable');
            if (table) {
                try {
                    // Store original table state
                    const originalTableHTML = table.innerHTML;

                    // Remove action column
                    const actionHeader = table.querySelector('th:last-child');
                    if (actionHeader) actionHeader.remove();

                    // Remove action cells
                    table.querySelectorAll('td:last-child').forEach(cell => cell.remove());

                    // Ensure all rows are visible for print
                    table.querySelectorAll('tr.table-secondary').forEach(row => {
                        row.classList.remove('table-secondary', 'opacity-75');
                    });

                    // Print
                    window.print();

                    // Restore table after printing
                    table.innerHTML = originalTableHTML;
                } catch (error) {
                    console.error('Error during print preparation:', error);
                    // If something goes wrong, just print as is
                    window.print();
                }
            } else {
                // If table is not found, print the page as is
                window.print();
            }

            // Remove print data attributes
            dashboardContent.removeAttribute('data-farmer-name');
            dashboardContent.removeAttribute('data-farmer-location');
            dashboardContent.removeAttribute('data-print-date');
        });
    }

    // Export to CSV functionality
    document.getElementById('exportBtn').addEventListener('click', function () {
        // Get table data
        const table = document.getElementById('productsTable');
        const rows = table.querySelectorAll('tr');

        let csvContent = "data:text/csv;charset=utf-8,";

        // Get headers
        const headers = [];
        const headerCells = rows[0].querySelectorAll('th');
        headerCells.forEach(cell => {
            // Skip the Actions column
            if (cell.textContent.trim() !== 'Actions') {
                headers.push('"' + cell.textContent.trim() + '"');
            }
        });
        csvContent += headers.join(',') + '\r\n';

        // Get rows
        for (let i = 1; i < rows.length; i++) {
            const row = rows[i];
            const cells = row.querySelectorAll('td');
            const rowData = [];

            cells.forEach((cell, index) => {
                // Skip the Actions column
                if (index < cells.length - 1) {
                    // Get text content, not HTML
                    let cellText = '';

                    // Special handling for badges
                    if (cell.querySelector('.badge')) {
                        cellText = cell.querySelector('.badge').textContent.trim();
                    } else {
                        cellText = cell.textContent.trim();
                    }

                    // Escape commas
                    cellText = '"' + cellText.replace(/"/g, '""') + '"';
                    rowData.push(cellText);
                }
            });

            csvContent += rowData.join(',') + '\r\n';
        }

        // Create download link
        const encodedUri = encodeURI(csvContent);
        const link = document.createElement('a');
        link.setAttribute('href', encodedUri);
        link.setAttribute('download', 'farmer_products.csv');
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    });

    // Filter collapse toggle
    const filterCollapse = document.getElementById('filterCollapse');
    const filterToggle = document.querySelector('[data-bs-target="#filterCollapse"]');

    if (filterCollapse && filterToggle) {
        filterToggle.addEventListener('click', function () {
            const icon = this.querySelector('i');
            if (icon) {
                icon.className = filterCollapse.classList.contains('show') ?
                    'fa fa-chevron-down' : 'fa fa-chevron-up';
            }
        });
    }

    // Form submission loading state
    const filterForm = document.getElementById('filterForm');
    if (filterForm) {
        filterForm.addEventListener('submit', function () {
            const submitBtn = this.querySelector('button[type="submit"]');
            if (submitBtn) {
                submitBtn.disabled = true;
                submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Applying...';
            }
        });
    }

    // Add loading state to action buttons
    document.querySelectorAll('form[method="post"]').forEach(form => {
        form.addEventListener('submit', function () {
            const submitBtn = this.querySelector('button[type="submit"]');
            if (submitBtn) {
                submitBtn.disabled = true;
                const originalContent = submitBtn.innerHTML;
                submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>';

                // Store original content for potential restoration
                submitBtn.dataset.originalContent = originalContent;
            }
        });
    });
}); 