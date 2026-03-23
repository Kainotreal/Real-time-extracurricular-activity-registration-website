// Custom Notification Toast System
class CustomToast {
    constructor() {
        this.init();
    }

    init() {
        // Override default confirm
        window.originalConfirm = window.confirm;
        window.confirm = (message) => {
            return this.showConfirm(message);
        };
    }

    showConfirm(message, onConfirm, onCancel) {
        return new Promise((resolve) => {
            // Tạo overlay
            const overlay = document.createElement('div');
            overlay.style.cssText = `
                position: fixed;
                top: 0;
                left: 0;
                width: 100%;
                height: 100%;
                background: rgba(0, 0, 0, 0.5);
                z-index: 9999;
                display: flex;
                align-items: center;
                justify-content: center;
                animation: fadeIn 0.3s ease;
            `;
            
            // Tạo notification box
            const notification = document.createElement('div');
            notification.style.cssText = `
                background: var(--pixel-white);
                border: var(--pixel-border);
                box-shadow: var(--pixel-shadow);
                padding: 30px;
                border-radius: 15px;
                max-width: 400px;
                text-align: center;
                font-family: 'VT323', monospace;
                position: relative;
                animation: slideIn 0.3s ease;
            `;
            
            // Icon
            const icon = document.createElement('div');
            icon.innerHTML = '<i class="fas fa-question-circle" style="font-size: 3rem; color: var(--pixel-blue-dark); margin-bottom: 15px;"></i>';
            
            // Message
            const messageDiv = document.createElement('div');
            messageDiv.style.cssText = `
                font-size: 1.2rem;
                color: var(--pixel-dark);
                margin-bottom: 25px;
                line-height: 1.4;
            `;
            messageDiv.textContent = message;
            
            // Buttons container
            const buttonsDiv = document.createElement('div');
            buttonsDiv.style.cssText = `
                display: flex;
                gap: 15px;
                justify-content: center;
            `;
            
            // Confirm button
            const confirmBtn = document.createElement('button');
            confirmBtn.style.cssText = `
                background: var(--pixel-green);
                color: var(--pixel-white);
                border: var(--pixel-border);
                box-shadow: var(--pixel-shadow);
                padding: 12px 25px;
                font-size: 1rem;
                font-weight: bold;
                cursor: pointer;
                transition: all 0.3s ease;
                font-family: 'VT323', monospace;
            `;
            confirmBtn.textContent = 'XÁC NHẬN';
            confirmBtn.onclick = function() {
                document.body.removeChild(overlay);
                if (onConfirm) onConfirm();
                resolve(true);
            };
            confirmBtn.onmouseover = function() {
                this.style.transform = 'translateY(-2px)';
                this.style.boxShadow = '4px 4px 0px var(--pixel-black)';
            };
            confirmBtn.onmouseout = function() {
                this.style.transform = 'translateY(0)';
                this.style.boxShadow = 'var(--pixel-shadow)';
            };
            
            // Cancel button
            const cancelBtn = document.createElement('button');
            cancelBtn.style.cssText = `
                background: var(--pixel-gray);
                color: var(--pixel-white);
                border: var(--pixel-border);
                box-shadow: var(--pixel-shadow);
                padding: 12px 25px;
                font-size: 1rem;
                font-weight: bold;
                cursor: pointer;
                transition: all 0.3s ease;
                font-family: 'VT323', monospace;
            `;
            cancelBtn.textContent = 'HỦY';
            cancelBtn.onclick = function() {
                document.body.removeChild(overlay);
                if (onCancel) onCancel();
                resolve(false);
            };
            cancelBtn.onmouseover = function() {
                this.style.transform = 'translateY(-2px)';
                this.style.boxShadow = '4px 4px 0px var(--pixel-black)';
            };
            cancelBtn.onmouseout = function() {
                this.style.transform = 'translateY(0)';
                this.style.boxShadow = 'var(--pixel-shadow)';
            };
            
            // Assembly
            buttonsDiv.appendChild(confirmBtn);
            buttonsDiv.appendChild(cancelBtn);
            notification.appendChild(icon);
            notification.appendChild(messageDiv);
            notification.appendChild(buttonsDiv);
            overlay.appendChild(notification);
            document.body.appendChild(overlay);
            
            // Focus on confirm button
            confirmBtn.focus();
            
            // Close on escape
            overlay.addEventListener('keydown', function(e) {
                if (e.key === 'Escape') {
                    document.body.removeChild(overlay);
                    if (onCancel) onCancel();
                    resolve(false);
                }
            });

            // Close on overlay click
            overlay.addEventListener('click', function(e) {
                if (e.target === overlay) {
                    document.body.removeChild(overlay);
                    if (onCancel) onCancel();
                    resolve(false);
                }
            });
        });
    }

    showAlert(message, type = 'info') {
        return new Promise((resolve) => {
            // Tạo overlay
            const overlay = document.createElement('div');
            overlay.style.cssText = `
                position: fixed;
                top: 0;
                left: 0;
                width: 100%;
                height: 100%;
                background: rgba(0, 0, 0, 0.5);
                z-index: 9999;
                display: flex;
                align-items: center;
                justify-content: center;
                animation: fadeIn 0.3s ease;
            `;
            
            // Tạo notification box
            const notification = document.createElement('div');
            notification.style.cssText = `
                background: var(--pixel-white);
                border: var(--pixel-border);
                box-shadow: var(--pixel-shadow);
                padding: 30px;
                border-radius: 15px;
                max-width: 400px;
                text-align: center;
                font-family: 'VT323', monospace;
                position: relative;
                animation: slideIn 0.3s ease;
            `;
            
            // Icon based on type
            const iconMap = {
                'info': 'fa-info-circle',
                'success': 'fa-check-circle',
                'warning': 'fa-exclamation-triangle',
                'error': 'fa-times-circle'
            };
            const colorMap = {
                'info': 'var(--pixel-blue-dark)',
                'success': 'var(--pixel-green)',
                'warning': 'var(--pixel-orange)',
                'error': 'var(--pixel-red)'
            };
            
            const icon = document.createElement('div');
            icon.innerHTML = `<i class="fas ${iconMap[type] || iconMap.info}" style="font-size: 3rem; color: ${colorMap[type] || colorMap.info}; margin-bottom: 15px;"></i>`;
            
            // Message
            const messageDiv = document.createElement('div');
            messageDiv.style.cssText = `
                font-size: 1.2rem;
                color: var(--pixel-dark);
                margin-bottom: 25px;
                line-height: 1.4;
            `;
            messageDiv.textContent = message;
            
            // OK button
            const okBtn = document.createElement('button');
            okBtn.style.cssText = `
                background: var(--pixel-blue-dark);
                color: var(--pixel-white);
                border: var(--pixel-border);
                box-shadow: var(--pixel-shadow);
                padding: 12px 25px;
                font-size: 1rem;
                font-weight: bold;
                cursor: pointer;
                transition: all 0.3s ease;
                font-family: 'VT323', monospace;
            `;
            okBtn.textContent = 'OK';
            okBtn.onclick = function() {
                document.body.removeChild(overlay);
                resolve();
            };
            okBtn.onmouseover = function() {
                this.style.transform = 'translateY(-2px)';
                this.style.boxShadow = '4px 4px 0px var(--pixel-black)';
            };
            okBtn.onmouseout = function() {
                this.style.transform = 'translateY(0)';
                this.style.boxShadow = 'var(--pixel-shadow)';
            };
            
            // Assembly
            notification.appendChild(icon);
            notification.appendChild(messageDiv);
            notification.appendChild(okBtn);
            overlay.appendChild(notification);
            document.body.appendChild(overlay);
            
            // Focus on OK button
            okBtn.focus();
            
            // Close on escape
            overlay.addEventListener('keydown', function(e) {
                if (e.key === 'Escape' || e.key === 'Enter') {
                    document.body.removeChild(overlay);
                    resolve();
                }
            });

            // Close on overlay click
            overlay.addEventListener('click', function(e) {
                if (e.target === overlay) {
                    document.body.removeChild(overlay);
                    resolve();
                }
            });
        });
    }
}

// CSS Animations
const style = document.createElement('style');
style.textContent = `
    @keyframes fadeIn {
        from { opacity: 0; }
        to { opacity: 1; }
    }
    
    @keyframes slideIn {
        from { 
            opacity: 0;
            transform: translateY(-20px) scale(0.9);
        }
        to { 
            opacity: 1;
            transform: translateY(0) scale(1);
        }
    }
`;
document.head.appendChild(style);

// Initialize custom toast
const customToast = new CustomToast();

// Global functions
window.showCustomConfirm = (message, onConfirm, onCancel) => {
    return customToast.showConfirm(message, onConfirm, onCancel);
};

window.showCustomAlert = (message, type) => {
    return customToast.showAlert(message, type);
};

// Global showToast function for compatibility
window.showToast = (type, message) => {
    // Create toast element
    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;
    toast.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background: ${type === 'success' ? '#28a745' : type === 'error' ? '#dc3545' : type === 'warning' ? '#ffc107' : '#17a2b8'};
        color: white;
        padding: 15px 20px;
        border-radius: 5px;
        box-shadow: 0 4px 12px rgba(0,0,0,0.15);
        z-index: 9999;
        transform: translateX(100%);
        transition: transform 0.3s ease;
        font-family: 'VT323', monospace;
        font-size: 1rem;
        max-width: 400px;
        word-wrap: break-word;
    `;
    toast.textContent = message;
    
    // Add to page
    document.body.appendChild(toast);
    
    // Animate in
    setTimeout(() => {
        toast.style.transform = 'translateX(0)';
    }, 10);
    
    // Remove after 5 seconds (tăng thời gian hiển thị)
    setTimeout(() => {
        toast.style.transform = 'translateX(100%)';
        setTimeout(() => {
            if (toast.parentNode) {
                toast.parentNode.removeChild(toast);
            }
        }, 300);
    }, 5000);
};

// Override onclick handlers that use confirm
document.addEventListener('DOMContentLoaded', function() {
    // Find all elements with onclick that contains confirm
    const elements = document.querySelectorAll('[onclick*="confirm"]');
    elements.forEach(element => {
        const onclick = element.getAttribute('onclick');
        if (onclick && onclick.includes('confirm')) {
            // Replace confirm with custom confirm
            const newOnclick = onclick.replace(/confirm\(/g, 'showCustomConfirm(');
            element.setAttribute('onclick', newOnclick);
        }
    });
}); 