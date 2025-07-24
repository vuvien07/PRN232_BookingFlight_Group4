/**
 * Feedback Pagination JavaScript Module
 * Handles client-side pagination for feedback lists
 * Supports search/filter functionality
 * Author: Copilot Assistant
 * Date: 2025
 */

class FeedbackPagination {
    constructor(options = {}) {
        this.itemsPerPage = options.itemsPerPage || 6;
        this.containerSelector = options.containerSelector || '.feedback-grid';
        this.itemSelector = options.itemSelector || '.feedback-card';
        this.searchInputId = options.searchInputId || 'searchText';
        this.ratingSelectId = options.ratingSelectId || 'rating';
        this.paginationId = options.paginationId || 'js-pagination';
        this.showAnimation = options.showAnimation !== false;
        this.scrollToTop = options.scrollToTop !== false;
        
        this.currentPage = 1;
        this.totalItems = 0;
        this.totalPages = 0;
        this.allItems = [];
        this.filteredItems = [];
        
        this.init();
    }

    init() {
        // Get all feedback items
        this.allItems = Array.from(document.querySelectorAll(this.itemSelector));
        this.filteredItems = [...this.allItems];
        this.totalItems = this.filteredItems.length;
        this.totalPages = Math.ceil(this.totalItems / this.itemsPerPage);
        
        if (this.totalItems > this.itemsPerPage) {
            this.createPaginationControls();
            this.showPage(1);
        } else {
            this.hideExistingPagination();
            this.showAllItems();
        }
    }

    hideExistingPagination() {
        // Hide server-side pagination if exists
        const existingPagination = document.querySelector('.pagination-container');
        if (existingPagination) {
            existingPagination.style.display = 'none';
        }
    }

    createPaginationControls() {
        // Remove existing JS pagination
        const existingPagination = document.querySelector('.js-pagination-container');
        if (existingPagination) {
            existingPagination.remove();
        }

        // Create new pagination container
        const paginationHTML = this.generatePaginationHTML();
        const container = document.querySelector(this.containerSelector);
        if (container) {
            container.insertAdjacentHTML('afterend', paginationHTML);
            this.attachPaginationEvents();
        }
    }

    generatePaginationHTML() {
        const startItem = (this.currentPage - 1) * this.itemsPerPage + 1;
        const endItem = Math.min(this.currentPage * this.itemsPerPage, this.totalItems);
        
        let paginationHTML = `
            <div class="js-pagination-container">
                <div class="pagination-container">
                    <nav aria-label="Feedback Pagination">
                        <ul class="pagination" id="${this.paginationId}">
                            <!-- Previous Page -->
                            <li class="page-item ${this.currentPage === 1 ? 'disabled' : ''}">
                                <a class="page-link" href="#" data-page="${this.currentPage - 1}" ${this.currentPage === 1 ? 'tabindex="-1"' : ''}>
                                    <i class="fas fa-chevron-left"></i>
                                    <span data-vn="Trước" data-en="Previous" data-zh="上一页">Trước</span>
                                </a>
                            </li>`;

        // Page numbers logic
        const startPage = Math.max(1, this.currentPage - 2);
        const endPage = Math.min(this.totalPages, this.currentPage + 2);

        // First page and ellipsis
        if (startPage > 1) {
            paginationHTML += `
                <li class="page-item">
                    <a class="page-link" href="#" data-page="1">1</a>
                </li>`;
            if (startPage > 2) {
                paginationHTML += `
                    <li class="page-item disabled">
                        <span class="page-link">...</span>
                    </li>`;
            }
        }

        // Page number buttons
        for (let i = startPage; i <= endPage; i++) {
            paginationHTML += `
                <li class="page-item ${i === this.currentPage ? 'active' : ''}">
                    ${i === this.currentPage ? 
                        `<span class="page-link current">${i}</span>` : 
                        `<a class="page-link" href="#" data-page="${i}">${i}</a>`
                    }
                </li>`;
        }

        // Last page and ellipsis
        if (endPage < this.totalPages) {
            if (endPage < this.totalPages - 1) {
                paginationHTML += `
                    <li class="page-item disabled">
                        <span class="page-link">...</span>
                    </li>`;
            }
            paginationHTML += `
                <li class="page-item">
                    <a class="page-link" href="#" data-page="${this.totalPages}">${this.totalPages}</a>
                </li>`;
        }

        paginationHTML += `
                            <!-- Next Page -->
                            <li class="page-item ${this.currentPage === this.totalPages ? 'disabled' : ''}">
                                <a class="page-link" href="#" data-page="${this.currentPage + 1}" ${this.currentPage === this.totalPages ? 'tabindex="-1"' : ''}>
                                    <span data-vn="Sau" data-en="Next" data-zh="下一页">Sau</span>
                                    <i class="fas fa-chevron-right"></i>
                                </a>
                            </li>
                        </ul>
                    </nav>
                    
                    <!-- Pagination Info -->
                    <div class="pagination-info">
                        <span data-vn="Hiển thị" data-en="Showing" data-zh="显示">Hiển thị</span>
                        <strong>${startItem}</strong>
                        -
                        <strong>${endItem}</strong>
                        <span data-vn="trong tổng số" data-en="of" data-zh="共">trong tổng số</span>
                        <strong>${this.totalItems}</strong>
                        <span data-vn="feedback" data-en="feedbacks" data-zh="反馈">feedback</span>
                    </div>
                </div>
            </div>`;

        return paginationHTML;
    }

    attachPaginationEvents() {
        const paginationLinks = document.querySelectorAll(`#${this.paginationId} .page-link[data-page]`);
        paginationLinks.forEach(link => {
            link.addEventListener('click', (e) => {
                e.preventDefault();
                const page = parseInt(link.getAttribute('data-page'));
                if (page >= 1 && page <= this.totalPages && page !== this.currentPage) {
                    this.goToPage(page);
                }
            });
        });
    }

    goToPage(page) {
        this.currentPage = page;
        this.showPage(page);
        this.updatePaginationControls();
        
        // Scroll to top of container
        if (this.scrollToTop) {
            const container = document.querySelector(this.containerSelector);
            if (container) {
                container.scrollIntoView({ behavior: 'smooth', block: 'start' });
            }
        }
    }

    showPage(page) {
        const startIndex = (page - 1) * this.itemsPerPage;
        const endIndex = startIndex + this.itemsPerPage;

        // Hide all items
        this.allItems.forEach(item => {
            item.style.display = 'none';
        });

        // Show items for current page
        const itemsToShow = this.filteredItems.slice(startIndex, endIndex);
        
        if (this.showAnimation) {
            // Show with animation
            itemsToShow.forEach((item, index) => {
                item.style.display = 'block';
                item.style.opacity = '0';
                item.style.transform = 'translateY(20px)';
                
                // Staggered animation
                setTimeout(() => {
                    item.style.transition = 'all 0.3s ease';
                    item.style.opacity = '1';
                    item.style.transform = 'translateY(0)';
                }, index * 100 + 50);
            });
        } else {
            // Show without animation
            itemsToShow.forEach(item => {
                item.style.display = 'block';
                item.style.opacity = '1';
                item.style.transform = 'translateY(0)';
            });
        }
    }

    showAllItems() {
        this.allItems.forEach(item => {
            item.style.display = 'block';
            item.style.opacity = '1';
            item.style.transform = 'translateY(0)';
        });
    }

    updatePaginationControls() {
        const existingPagination = document.querySelector('.js-pagination-container');
        if (existingPagination) {
            existingPagination.remove();
        }
        this.createPaginationControls();
    }

    // Method to handle search/filter
    applyFilter(searchText = '', rating = '') {
        this.filteredItems = this.allItems.filter(item => {
            const title = item.querySelector('.feedback-title')?.textContent.toLowerCase() || '';
            const customerName = item.querySelector('.feedback-author span')?.textContent.toLowerCase() || '';
            const flightNumber = item.querySelector('.detail-value')?.textContent.toLowerCase() || '';
            const stars = item.querySelectorAll('.feedback-rating .star:not(.empty)').length;

            const matchesSearch = !searchText || 
                title.includes(searchText.toLowerCase()) || 
                customerName.includes(searchText.toLowerCase()) ||
                flightNumber.includes(searchText.toLowerCase());
            
            const matchesRating = !rating || stars == parseInt(rating);

            return matchesSearch && matchesRating;
        });

        this.totalItems = this.filteredItems.length;
        this.totalPages = Math.ceil(this.totalItems / this.itemsPerPage);
        this.currentPage = 1;

        if (this.totalItems > this.itemsPerPage) {
            this.createPaginationControls();
            this.showPage(1);
        } else {
            const existingPagination = document.querySelector('.js-pagination-container');
            if (existingPagination) {
                existingPagination.style.display = 'none';
            }
            this.showFilteredItems();
        }
    }

    showFilteredItems() {
        // Hide all items first
        this.allItems.forEach(item => {
            item.style.display = 'none';
        });

        // Show filtered items
        if (this.showAnimation) {
            this.filteredItems.forEach((item, index) => {
                item.style.display = 'block';
                item.style.opacity = '0';
                item.style.transform = 'translateY(20px)';
                
                setTimeout(() => {
                    item.style.transition = 'all 0.3s ease';
                    item.style.opacity = '1';
                    item.style.transform = 'translateY(0)';
                }, index * 50 + 50);
            });
        } else {
            this.filteredItems.forEach(item => {
                item.style.display = 'block';
                item.style.opacity = '1';
                item.style.transform = 'translateY(0)';
            });
        }
    }

    // Method to set up search form integration
    setupSearchForm(formSelector = '.search-filter-form') {
        const searchForm = document.querySelector(formSelector);
        if (!searchForm) return;

        searchForm.addEventListener('submit', (e) => {
            e.preventDefault();
            
            const searchText = document.getElementById(this.searchInputId)?.value || '';
            const rating = document.getElementById(this.ratingSelectId)?.value || '';
            
            this.applyFilter(searchText, rating);
        });

        // Handle reset button
        const resetButton = searchForm.querySelector('.btn-outline-secondary');
        if (resetButton) {
            resetButton.addEventListener('click', (e) => {
                e.preventDefault();
                
                // Clear form
                const searchInput = document.getElementById(this.searchInputId);
                const ratingSelect = document.getElementById(this.ratingSelectId);
                
                if (searchInput) searchInput.value = '';
                if (ratingSelect) ratingSelect.value = '';
                
                // Reset filter
                this.applyFilter('', '');
            });
        }
    }

    // Method to refresh pagination (useful after dynamic content changes)
    refresh() {
        this.init();
    }
}

// Export for use in other scripts
window.FeedbackPagination = FeedbackPagination;
