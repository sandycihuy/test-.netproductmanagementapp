// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Wait for the DOM to be fully loaded
document.addEventListener('DOMContentLoaded', function() {
    // Initialize tooltips
    initTooltips();
    
    // Initialize dark/light mode
    initThemeToggle();
    
    // Initialize form validation
    initFormValidation();
    
    // Initialize image preview for file inputs
    initImagePreview();
});

// Initialize Bootstrap tooltips
function initTooltips() {
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
}

// Initialize theme toggle functionality
function initThemeToggle() {
    const themeToggle = document.getElementById('theme-toggle');
    if (!themeToggle) return;
    
    // Check for saved user preference, if any, on load
    const savedTheme = localStorage.getItem('theme') || 'light';
    setTheme(savedTheme);
    
    // Toggle theme on button click
    themeToggle.addEventListener('click', function() {
        const currentTheme = document.documentElement.getAttribute('data-bs-theme') || 'light';
        const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
        setTheme(newTheme);
        localStorage.setItem('theme', newTheme);
    });
    
    // Update button icon based on theme
    function updateThemeIcon(theme) {
        const icon = themeToggle.querySelector('i');
        if (!icon) return;
        
        if (theme === 'dark') {
            icon.classList.remove('fa-moon');
            icon.classList.add('fa-sun');
        } else {
            icon.classList.remove('fa-sun');
            icon.classList.add('fa-moon');
        }
    }
    
    // Apply theme
    function setTheme(theme) {
        document.documentElement.setAttribute('data-bs-theme', theme);
        updateThemeIcon(theme);
    }
}

// Initialize form validation
function initFormValidation() {
    // Fetch all forms that need validation
    const forms = document.querySelectorAll('.needs-validation');
    
    // Loop over them and prevent submission
    Array.from(forms).forEach(form => {
        form.addEventListener('submit', event => {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }
            
            form.classList.add('was-validated');
        }, false);
    });
}

// Initialize image preview for file inputs
function initImagePreview() {
    const imageInputs = document.querySelectorAll('.image-upload');
    
    imageInputs.forEach(input => {
        const previewContainer = input.closest('.image-upload-container');
        if (!previewContainer) return;
        
        const preview = previewContainer.querySelector('.image-preview');
        const removeBtn = previewContainer.querySelector('.remove-image');
        const defaultImage = previewContainer.dataset.defaultImage;
        
        // Show/hide remove button based on whether an image is selected
        function updateRemoveButton() {
            if (!removeBtn) return;
            
            if (input.files && input.files[0]) {
                removeBtn.classList.remove('d-none');
            } else if (!preview.src.includes(defaultImage)) {
                removeBtn.classList.remove('d-none');
            } else {
                removeBtn.classList.add('d-none');
            }
        }
        
        // Update image preview
        function updatePreview() {
            if (input.files && input.files[0]) {
                const reader = new FileReader();
                
                reader.onload = function(e) {
                    preview.src = e.target.result;
                    preview.classList.remove('d-none');
                    updateRemoveButton();
                };
                
                reader.readAsDataURL(input.files[0]);
            } else if (defaultImage) {
                preview.src = defaultImage;
                preview.classList.remove('d-none');
                updateRemoveButton();
            } else {
                preview.src = '#';
                preview.classList.add('d-none');
                updateRemoveButton();
            }
        }
        
        // Handle image removal
        if (removeBtn) {
            removeBtn.addEventListener('click', function(e) {
                e.preventDefault();
                input.value = '';
                
                if (defaultImage) {
                    preview.src = defaultImage;
                } else {
                    preview.src = '#';
                    preview.classList.add('d-none');
                }
                
                updateRemoveButton();
            });
        }
        
        // Update preview when file is selected
        input.addEventListener('change', updatePreview);
        
        // Initial update
        updatePreview();
    });
}

// Show loading state for buttons
function setButtonLoading(button, isLoading) {
    if (!button) return;
    
    if (isLoading) {
        button.disabled = true;
        const originalText = button.innerHTML;
        button.setAttribute('data-original-text', originalText);
        button.innerHTML = `
            <span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
            Loading...
        `;
    } else {
        button.disabled = false;
        const originalText = button.getAttribute('data-original-text');
        if (originalText) {
            button.innerHTML = originalText;
            button.removeAttribute('data-original-text');
        }
    }
}

// Show confirmation modal
document.addEventListener('click', function(e) {
    if (e.target.matches('[data-confirm]')) {
        e.preventDefault();
        const message = e.target.getAttribute('data-confirm') || 'Are you sure you want to perform this action?';
        
        // You can use Bootstrap's built-in modal or a simple confirm dialog
        if (confirm(message)) {
            // If it's a form, submit it
            if (e.target.form) {
                e.target.form.submit();
            } 
            // If it's a link, navigate to the URL
            else if (e.target.href) {
                window.location.href = e.target.href;
            }
        }
    }
});

// Helper function to show toast notifications
function showToast(type, message, title = '') {
    const toastContainer = document.createElement('div');
    toastContainer.className = 'toast-container position-fixed bottom-0 end-0 p-3';
    toastContainer.style.zIndex = '1100';
    
    const toast = document.createElement('div');
    toast.className = 'toast show';
    toast.role = 'alert';
    toast.setAttribute('aria-live', 'assertive');
    toast.setAttribute('aria-atomic', 'true');
    
    const toastHeader = document.createElement('div');
    toastHeader.className = `toast-header bg-${type} text-white`;
    
    const toastTitle = document.createElement('strong');
    toastTitle.className = 'me-auto';
    toastTitle.textContent = title || type.charAt(0).toUpperCase() + type.slice(1);
    
    const closeButton = document.createElement('button');
    closeButton.type = 'button';
    closeButton.className = 'btn-close btn-close-white';
    closeButton.setAttribute('data-bs-dismiss', 'toast');
    closeButton.setAttribute('aria-label', 'Close');
    
    const toastBody = document.createElement('div');
    toastBody.className = 'toast-body';
    toastBody.textContent = message;
    
    toastHeader.appendChild(toastTitle);
    toastHeader.appendChild(closeButton);
    toast.appendChild(toastHeader);
    toast.appendChild(toastBody);
    toastContainer.appendChild(toast);
    
    document.body.appendChild(toastContainer);
    
    // Auto-remove after delay
    setTimeout(() => {
        toast.classList.remove('show');
        setTimeout(() => {
            toastContainer.remove();
        }, 300);
    }, 5000);
    
    // Add click handler to close button
    closeButton.addEventListener('click', () => {
        toast.classList.remove('show');
        setTimeout(() => {
            toastContainer.remove();
        }, 300);
    });
}
