// News Details Page JavaScript
class NewsDetailsManager {
    constructor(options = {}) {
        this.currentArticleId = options.currentArticleId || 1;
        this.currentCategory = options.currentCategory || '';
        this.init();
    }

    init() {
        this.setupReadingProgress();
        this.setupSmoothAnimations();
        this.setupShareFunctions();
        this.setupImageEnhancements();
        this.setupScrollEffects();
        this.loadRelatedNews();
    }

    // Reading progress indicator
    setupReadingProgress() {
        const progressBar = document.createElement('div');
        progressBar.className = 'reading-progress';
        document.body.appendChild(progressBar);

        window.addEventListener('scroll', () => {
            const scrollTop = window.pageYOffset;
            const docHeight = document.body.scrollHeight - window.innerHeight;
            const scrollPercent = (scrollTop / docHeight) * 100;
            progressBar.style.width = Math.min(scrollPercent, 100) + '%';
        });
    }

    // Smooth page animation
    setupSmoothAnimations() {
        window.addEventListener('load', () => {
            document.body.style.opacity = '0';
            document.body.style.transition = 'opacity 0.5s ease';
            
            setTimeout(() => {
                document.body.style.opacity = '1';
            }, 100);
        });
    }

    // Share functions
    setupShareFunctions() {
        window.shareOnFacebook = () => {
            const url = encodeURIComponent(window.location.href);
            window.open(`https://www.facebook.com/sharer/sharer.php?u=${url}`, '_blank', 'width=600,height=400');
        };

        window.shareOnTwitter = () => {
            const url = encodeURIComponent(window.location.href);
            const title = document.querySelector('.article-title')?.textContent || '';
            const text = encodeURIComponent(`${title} ${url}`);
            window.open(`https://twitter.com/intent/tweet?text=${text}`, '_blank', 'width=600,height=400');
        };

        window.shareOnLinkedIn = () => {
            const url = encodeURIComponent(window.location.href);
            window.open(`https://www.linkedin.com/sharing/share-offsite/?url=${url}`, '_blank', 'width=600,height=400');
        };

        // Attach event listeners to share buttons
        document.querySelectorAll('[data-platform]').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const platform = e.currentTarget.getAttribute('data-platform');
                switch(platform) {
                    case 'facebook':
                        window.shareOnFacebook();
                        break;
                    case 'twitter':
                        window.shareOnTwitter();
                        break;
                    case 'linkedin':
                        window.shareOnLinkedIn();
                        break;
                }
            });
        });
    }

    // Load related news articles
    async loadRelatedNews() {
        const relatedGrid = document.getElementById('relatedNews');
        if (!relatedGrid) return;

        try {
            // Tạo sample data nếu API chưa sẵn sàng
            const relatedNews = [
                {
                    NewId: 2,
                    Title: "Máy bay Hàn Quốc xuất kích theo sát chiến đấu cơ Nga",
                    ImageUrl: "/images/news/han-quoc-may-bay.jpg",
                    PublishedDate: "2025-07-19"
                },
                {
                    NewId: 3,
                    Title: "Chương trình khuyến mãi đặc biệt Xuân 2025",
                    ImageUrl: "/images/news/khuyen-mai-xuan.jpg",
                    PublishedDate: "2025-07-18"
                },
                {
                    NewId: 4,
                    Title: "Cập nhật lịch bay mới cho tuyến quốc tế",
                    ImageUrl: "/images/news/lich-bay-moi.jpg",
                    PublishedDate: "2025-07-17"
                }
            ];

            relatedGrid.innerHTML = relatedNews.map(news => `
                <div class="related-item" onclick="location.href='/News/Details/${news.NewId}'">
                    <img src="${news.ImageUrl}" alt="${news.Title}" class="related-image" loading="lazy">
                    <div class="related-content">
                        <h4 class="related-item-title">${news.Title}</h4>
                        <div class="related-date">${new Date(news.PublishedDate).toLocaleDateString('vi-VN')}</div>
                    </div>
                </div>
            `).join('');
        } catch (error) {
            console.log('Related news load error:', error);
            relatedGrid.innerHTML = '<p style="text-align: center; color: #6c757d;">Không thể tải tin tức liên quan</p>';
        }
    }

    // Image enhancements
    setupImageEnhancements() {
        const articleImages = document.querySelectorAll('.featured-image img, .article-content img');
        
        articleImages.forEach(img => {
            // Add loading state
            img.addEventListener('load', () => {
                img.style.opacity = '1';
            });

            // Add click to zoom
            img.addEventListener('click', () => {
                this.openImageModal(img.src, img.alt);
            });

            img.style.cursor = 'pointer';
            img.style.transition = 'opacity 0.3s ease';
        });
    }

    // Image modal
    openImageModal(src, alt) {
        const modal = document.createElement('div');
        modal.style.cssText = `
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0,0,0,0.9);
            display: flex;
            justify-content: center;
            align-items: center;
            z-index: 10000;
            cursor: pointer;
        `;

        const img = document.createElement('img');
        img.src = src;
        img.alt = alt;
        img.style.cssText = `
            max-width: 90%;
            max-height: 90%;
            object-fit: contain;
            border-radius: 10px;
        `;

        modal.appendChild(img);
        document.body.appendChild(modal);

        // Close on click
        modal.addEventListener('click', () => {
            modal.remove();
        });

        // Close on ESC
        const handleEsc = (e) => {
            if (e.key === 'Escape') {
                modal.remove();
                document.removeEventListener('keydown', handleEsc);
            }
        };
        document.addEventListener('keydown', handleEsc);
    }

    // Scroll effects
    setupScrollEffects() {
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.style.opacity = '1';
                    entry.target.style.transform = 'translateY(0)';
                }
            });
        }, { threshold: 0.1 });

        // Observe elements
        document.querySelectorAll('.related-item, .article-content, .article-actions').forEach(el => {
            el.style.opacity = '0';
            el.style.transform = 'translateY(30px)';
            el.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
            observer.observe(el);
        });
    }

    // Calculate reading time
    calculateReadingTime() {
        const content = document.querySelector('.article-content');
        if (!content) return;

        const text = content.textContent || content.innerText || '';
        const wordsPerMinute = 200;
        const words = text.trim().split(/\s+/).length;
        const readingTime = Math.ceil(words / wordsPerMinute);

        // Update reading time displays
        document.querySelectorAll('.read-time').forEach(el => {
            el.innerHTML = `<i class="fas fa-clock"></i> ${readingTime} phút đọc`;
        });
    }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    // Check if we have window.newsDetailsManager from inline script
    if (window.newsDetailsManager) {
        return; // Already initialized
    }
    
    // Initialize with default options
    const newsDetails = new NewsDetailsManager();
    newsDetails.calculateReadingTime();
});

// Export for potential module usage
if (typeof module !== 'undefined' && module.exports) {
    module.exports = NewsDetailsManager;
}
