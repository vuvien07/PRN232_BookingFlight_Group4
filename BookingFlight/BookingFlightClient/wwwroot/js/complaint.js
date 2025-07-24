// Complaint Module JavaScript

class ComplaintManager {
    constructor() {
        this.initializeEventListeners();
        this.setupFormValidation();
    }

    initializeEventListeners() {
        // Character counter for description
        const descriptionTextarea = document.getElementById('Description');
        if (descriptionTextarea) {
            descriptionTextarea.addEventListener('input', this.updateCharacterCount.bind(this));
        }

        // File upload handling
        const fileInput = document.getElementById('attachmentFile');
        if (fileInput) {
            fileInput.addEventListener('change', this.handleFileUpload.bind(this));
        }

        // Form submission
        const complaintForm = document.getElementById('complaintForm');
        if (complaintForm) {
            complaintForm.addEventListener('submit', this.handleFormSubmit.bind(this));
        }

        // Filter and sort functionality
        this.setupFilterAndSort();
    }

    updateCharacterCount() {
        const textarea = document.getElementById('Description');
        const counter = document.getElementById('charCount');
        
        if (textarea && counter) {
            const length = textarea.value.length;
            counter.textContent = length;
            
            // Change color based on character count
            if (length > 450) {
                counter.style.color = '#dc3545';
            } else if (length > 400) {
                counter.style.color = '#fd7e14';
            } else {
                counter.style.color = '#6c757d';
            }
        }
    }

    handleFileUpload(event) {
        const file = event.target.files[0];
        const fileInfo = document.getElementById('fileInfo');
        
        if (file && fileInfo) {
            const fileSize = (file.size / 1024 / 1024).toFixed(2);
            const fileName = file.name;
            const allowedExtensions = ['.jpg', '.jpeg', '.png', '.pdf', '.doc', '.docx'];
            const fileExtension = fileName.toLowerCase().substring(fileName.lastIndexOf('.'));
            
            if (!allowedExtensions.includes(fileExtension)) {
                this.showError('Chỉ chấp nhận file ảnh (jpg, png) hoặc tài liệu (pdf, doc, docx)');
                event.target.value = '';
                fileInfo.style.display = 'none';
                return;
            }
            
            if (fileSize > 5) {
                this.showError('File quá lớn! Vui lòng chọn file nhỏ hơn 5MB.');
                event.target.value = '';
                fileInfo.style.display = 'none';
                return;
            }
            
            fileInfo.innerHTML = `
                <i class="fas fa-file me-2"></i>
                <strong>${fileName}</strong> (${fileSize} MB)
            `;
            fileInfo.className = 'file-info alert-info';
            fileInfo.style.display = 'block';
        }
    }

    handleFormSubmit(event) {
        const submitBtn = document.getElementById('submitBtn');
        const submitText = document.getElementById('submitText');
        const loadingText = document.getElementById('loadingText');
        
        if (submitBtn && submitText && loadingText) {
            submitBtn.disabled = true;
            submitText.style.display = 'none';
            loadingText.style.display = 'inline';
        }
        
        // Form will submit normally, this just provides user feedback
        return true;
    }

    setupFormValidation() {
        const form = document.getElementById('complaintForm');
        if (form) {
            form.addEventListener('submit', (event) => {
                const description = document.getElementById('Description').value.trim();
                
                if (description.length < 10) {
                    event.preventDefault();
                    this.showError('Mô tả khiếu nại phải có ít nhất 10 ký tự.');
                    return false;
                }
                
                if (description.length > 500) {
                    event.preventDefault();
                    this.showError('Mô tả khiếu nại không được vượt quá 500 ký tự.');
                    return false;
                }
                
                return true;
            });
        }
    }

    setupFilterAndSort() {
        const statusFilter = document.getElementById('statusFilter');
        const sortFilter = document.getElementById('sortFilter');
        
        if (statusFilter) {
            statusFilter.addEventListener('change', this.filterComplaints.bind(this));
        }
        
        if (sortFilter) {
            sortFilter.addEventListener('change', this.sortComplaints.bind(this));
        }
    }

    filterComplaints() {
        const statusFilter = document.getElementById('statusFilter');
        const complaints = document.querySelectorAll('.complaint-item');
        
        if (statusFilter && complaints.length > 0) {
            const selectedStatus = statusFilter.value;
            
            complaints.forEach(complaint => {
                const status = complaint.getAttribute('data-status');
                if (selectedStatus === '' || status === selectedStatus) {
                    complaint.style.display = 'block';
                    complaint.classList.add('fade-in');
                } else {
                    complaint.style.display = 'none';
                }
            });
        }
    }

    sortComplaints() {
        const sortFilter = document.getElementById('sortFilter');
        const container = document.getElementById('complaintsContainer');
        
        if (sortFilter && container) {
            const complaints = Array.from(container.children);
            const sortBy = sortFilter.value;
            
            complaints.sort((a, b) => {
                if (sortBy === 'newest') {
                    const dateA = this.extractDate(a);
                    const dateB = this.extractDate(b);
                    return dateB - dateA;
                } else if (sortBy === 'oldest') {
                    const dateA = this.extractDate(a);
                    const dateB = this.extractDate(b);
                    return dateA - dateB;
                } else if (sortBy === 'status') {
                    const statusA = parseInt(a.getAttribute('data-status'));
                    const statusB = parseInt(b.getAttribute('data-status'));
                    return statusA - statusB;
                }
                return 0;
            });
            
            complaints.forEach(complaint => {
                container.appendChild(complaint);
                complaint.classList.add('slide-up');
            });
        }
    }

    extractDate(element) {
        const dateText = element.querySelector('.complaint-date').textContent;
        // Extract date from format like "📅 24/07/2025 14:30"
        const dateMatch = dateText.match(/(\d{2}\/\d{2}\/\d{4}\s\d{2}:\d{2})/);
        if (dateMatch) {
            const [day, month, year, time] = dateMatch[1].split(/[\/\s:]/);
            return new Date(`${year}-${month}-${day}T${time.slice(0,2)}:${time.slice(2)}`);
        }
        return new Date();
    }

    showError(message) {
        // Use SweetAlert2 if available, otherwise use alert
        if (typeof Swal !== 'undefined') {
            Swal.fire({
                icon: 'error',
                title: 'Lỗi',
                text: message,
                confirmButtonColor: '#dc3545'
            });
        } else {
            alert(message);
        }
    }

    showSuccess(message) {
        // Use SweetAlert2 if available, otherwise use alert
        if (typeof Swal !== 'undefined') {
            Swal.fire({
                icon: 'success',
                title: 'Thành công',
                text: message,
                confirmButtonColor: '#28a745'
            });
        } else {
            alert(message);
        }
    }

    // Utility function to format date
    formatDate(date) {
        if (!(date instanceof Date)) return '';
        
        const day = date.getDate().toString().padStart(2, '0');
        const month = (date.getMonth() + 1).toString().padStart(2, '0');
        const year = date.getFullYear();
        const hours = date.getHours().toString().padStart(2, '0');
        const minutes = date.getMinutes().toString().padStart(2, '0');
        
        return `${day}/${month}/${year} ${hours}:${minutes}`;
    }

    // Function to expand/collapse complaint descriptions
    toggleDescription(element) {
        element.classList.toggle('truncated');
    }

    // Function to copy complaint ID to clipboard
    async copyComplaintId(complaintId) {
        try {
            await navigator.clipboard.writeText(`#${complaintId}`);
            this.showSuccess('Đã sao chép mã khiếu nại vào clipboard');
        } catch (err) {
            // Fallback for older browsers
            const textArea = document.createElement('textarea');
            textArea.value = `#${complaintId}`;
            document.body.appendChild(textArea);
            textArea.select();
            document.execCommand('copy');
            document.body.removeChild(textArea);
            this.showSuccess('Đã sao chép mã khiếu nại vào clipboard');
        }
    }
}

// FAQ Toggle Function
function toggleFAQ(element) {
    const answer = element.nextElementSibling;
    const icon = element.querySelector('.fa-chevron-down');
    
    if (answer.style.display === 'none' || answer.style.display === '') {
        answer.style.display = 'block';
        icon.style.transform = 'rotate(180deg)';
        element.classList.add('active');
    } else {
        answer.style.display = 'none';
        icon.style.transform = 'rotate(0deg)';
        element.classList.remove('active');
    }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    // Initialize complaint manager
    const complaintManager = new ComplaintManager();
    
    // Make it globally available
    window.complaintManager = complaintManager;
    
    // Set up expandable descriptions
    document.querySelectorAll('.complaint-description').forEach(desc => {
        if (desc.scrollHeight > desc.clientHeight) {
            desc.style.cursor = 'pointer';
            desc.title = 'Click để xem đầy đủ';
            desc.addEventListener('click', function() {
                complaintManager.toggleDescription(this);
            });
        }
    });
    
    // Initialize character count if description field exists
    const descriptionField = document.getElementById('Description');
    if (descriptionField) {
        complaintManager.updateCharacterCount();
    }
});

// Export for use in other scripts
if (typeof module !== 'undefined' && module.exports) {
    module.exports = ComplaintManager;
}
