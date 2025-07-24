// News Index Page JavaScript
class NewsManager {
    constructor() {
        this.init();
    }

    init() {
        this.setupLanguageSwitching();
        this.setupSmoothScrolling();
        this.setupLoadingStates();
        this.setupLazyLoading();
        this.setupScrollAnimations();
        this.setupCardAnimations();
    }

    // Language switching support
    setupLanguageSwitching() {
        window.updateLanguage = (lang) => {
            document.querySelectorAll(`[data-${lang}]`).forEach(element => {
                const text = element.getAttribute(`data-${lang}`);
                if (text) {
                    element.textContent = text;
                }
            });
        };
    }

    // Smooth scrolling to top when page changes
    setupSmoothScrolling() {
        if (window.location.search.includes('page=')) {
            window.scrollTo({ top: 0, behavior: 'smooth' });
        }
    }

    // Add loading state for news cards
    setupLoadingStates() {
        document.querySelectorAll('.news-card').forEach(card => {
            card.addEventListener('click', () => {
                const loading = document.getElementById('loading');
                if (loading) {
                    loading.classList.add('show');
                }
            });
        });
    }

    // Lazy loading images
    setupLazyLoading() {
        if ('IntersectionObserver' in window) {
            const imageObserver = new IntersectionObserver((entries, observer) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        const img = entry.target;
                        img.src = img.getAttribute('data-src') || img.src;
                        img.classList.remove('lazy');
                        imageObserver.unobserve(img);
                    }
                });
            });

            document.querySelectorAll('img[loading="lazy"]').forEach(img => {
                imageObserver.observe(img);
            });
        }
    }

    // Add smooth animation on scroll
    setupScrollAnimations() {
        const animateCards = () => {
            const cards = document.querySelectorAll('.news-card');
            cards.forEach(card => {
                const cardTop = card.getBoundingClientRect().top;
                const cardVisible = 150;
                
                if (cardTop < window.innerHeight - cardVisible) {
                    card.style.opacity = '1';
                    card.style.transform = 'translateY(0)';
                }
            });
        };

        window.addEventListener('scroll', animateCards);
        
        // Trigger animation on load
        window.addEventListener('load', () => {
            setTimeout(animateCards, 100);
        });
    }

    // Initialize cards animation
    setupCardAnimations() {
        document.querySelectorAll('.news-card').forEach(card => {
            card.style.opacity = '0';
            card.style.transform = 'translateY(20px)';
            card.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
        });
    }

    // Search functionality (if needed in the future)
    setupSearch() {
        const searchInput = document.getElementById('newsSearch');
        if (searchInput) {
            searchInput.addEventListener('input', (e) => {
                this.filterNews(e.target.value);
            });
        }
    }

    filterNews(searchTerm) {
        const cards = document.querySelectorAll('.news-card');
        const lowerSearchTerm = searchTerm.toLowerCase();

        cards.forEach(card => {
            const title = card.querySelector('.news-card-title').textContent.toLowerCase();
            const summary = card.querySelector('.news-summary').textContent.toLowerCase();
            const category = card.querySelector('.news-category').textContent.toLowerCase();

            if (title.includes(lowerSearchTerm) || 
                summary.includes(lowerSearchTerm) || 
                category.includes(lowerSearchTerm)) {
                card.style.display = 'block';
                setTimeout(() => {
                    card.style.opacity = '1';
                    card.style.transform = 'translateY(0)';
                }, 50);
            } else {
                card.style.opacity = '0';
                card.style.transform = 'translateY(20px)';
                setTimeout(() => {
                    card.style.display = 'none';
                }, 300);
            }
        });
    }

    // Category filter functionality
    setupCategoryFilter() {
        const categoryButtons = document.querySelectorAll('.category-filter');
        categoryButtons.forEach(button => {
            button.addEventListener('click', (e) => {
                const category = e.target.dataset.category;
                this.filterByCategory(category);
                
                // Update active button
                categoryButtons.forEach(btn => btn.classList.remove('active'));
                e.target.classList.add('active');
            });
        });
    }

    filterByCategory(selectedCategory) {
        const cards = document.querySelectorAll('.news-card');
        
        cards.forEach(card => {
            const cardCategory = card.querySelector('.news-category').textContent.trim();
            
            if (selectedCategory === 'all' || cardCategory === selectedCategory) {
                card.style.display = 'block';
                setTimeout(() => {
                    card.style.opacity = '1';
                    card.style.transform = 'translateY(0)';
                }, 50);
            } else {
                card.style.opacity = '0';
                card.style.transform = 'translateY(20px)';
                setTimeout(() => {
                    card.style.display = 'none';
                }, 300);
            }
        });
    }

    // Performance monitoring
    measurePerformance() {
        if ('performance' in window) {
            window.addEventListener('load', () => {
                const perfData = performance.getEntriesByType('navigation')[0];
                console.log('Page load time:', perfData.loadEventEnd - perfData.loadEventStart, 'ms');
            });
        }
    }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    new NewsManager();
});

// Export for potential module usage
if (typeof module !== 'undefined' && module.exports) {
    module.exports = NewsManager;
}
