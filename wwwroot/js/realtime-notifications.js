// Realtime Notifications System
class RealtimeNotifications {
    constructor() {
        this.connection = null;
        this.notificationContainer = null;
        this.notificationCount = 0;
        this.maxNotifications = 5;
        this.userRoles = [];
        this.userId = null;
        this.isInitialized = false;
        this.init();
    }

    async init() {
        try {
            console.log("Initializing RealtimeNotifications...");
            this.setupNotificationContainer();
            await this.connectToHub();
            this.setupEventListeners();
            this.isInitialized = true;
            console.log("RealtimeNotifications initialized successfully");
        } catch (error) {
            console.error("Error initializing RealtimeNotifications:", error);
            // Fallback initialization
            this.setupNotificationContainer();
            this.isInitialized = true;
        }
    }

    setupNotificationContainer() {
        try {
            // Kiểm tra xem container đã tồn tại chưa
            this.notificationContainer = document.getElementById('realtime-notifications');
            if (!this.notificationContainer) {
                // Tạo container cho thông báo
                this.notificationContainer = document.createElement('div');
                this.notificationContainer.id = 'realtime-notifications';
                this.notificationContainer.className = 'notification-container';
                document.body.appendChild(this.notificationContainer);
            }

            // Thêm CSS nếu chưa có
            if (!document.getElementById('notification-styles')) {
                this.addNotificationStyles();
            }
        } catch (error) {
            console.error("Error setting up notification container:", error);
        }
    }

    addNotificationStyles() {
        try {
            // Kiểm tra xem CSS đã được load chưa
            const existingStyles = document.getElementById('notification-styles');
            if (existingStyles) {
                return; // CSS đã có sẵn
            }

            // Tạo style element để đảm bảo CSS được load
            const style = document.createElement('style');
            style.id = 'notification-styles';
            style.textContent = `
                /* Fallback styles nếu CSS file không load được */
                .notification-container {
                    position: fixed;
                    top: 20px;
                    right: 20px;
                    z-index: 9999;
                    max-width: 400px;
                    pointer-events: none;
                }

                .notification {
                    background: #ffffff;
                    border: 2px solid #e0e0e0;
                    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
                    border-radius: 8px;
                    padding: 16px;
                    margin-bottom: 12px;
                    transform: translateX(100%);
                    opacity: 0;
                    transition: all 0.4s cubic-bezier(0.68, -0.55, 0.265, 1.55);
                    pointer-events: auto;
                    position: relative;
                    overflow: hidden;
                    font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
                }

                .notification.show {
                    transform: translateX(0);
                    opacity: 1;
                }

                .notification.hide {
                    transform: translateX(100%);
                    opacity: 0;
                }

                .notification::before {
                    content: '';
                    position: absolute;
                    top: 0;
                    left: 0;
                    width: 4px;
                    height: 100%;
                    background: #4682B4;
                }

                .notification.success::before {
                    background: #28a745;
                }

                .notification.warning::before {
                    background: #ffc107;
                }

                .notification.error::before {
                    background: #dc3545;
                }

                .notification.info::before {
                    background: #17a2b8;
                }

                .notification-content {
                    display: flex;
                    align-items: flex-start;
                    gap: 12px;
                }

                .notification-icon {
                    flex-shrink: 0;
                    font-size: 20px;
                    margin-top: 2px;
                }

                .notification-body {
                    flex: 1;
                    min-width: 0;
                }

                .notification-header {
                    display: flex;
                    justify-content: space-between;
                    align-items: center;
                    margin-bottom: 8px;
                }

                .notification-title {
                    font-weight: 600;
                    font-size: 14px;
                    color: #2c3e50;
                    margin: 0;
                }

                .notification-time {
                    font-size: 11px;
                    color: #95a5a6;
                    font-weight: 500;
                }

                .notification-message {
                    font-size: 13px;
                    color: #34495e;
                    margin: 0;
                    line-height: 1.4;
                    word-wrap: break-word;
                }

                .notification-close {
                    background: none;
                    border: none;
                    color: #95a5a6;
                    cursor: pointer;
                    padding: 4px;
                    font-size: 12px;
                    transition: color 0.2s ease;
                    border-radius: 4px;
                }

                .notification-close:hover {
                    color: #dc3545;
                    background-color: #f8f9fa;
                }

                .notification-pulse {
                    animation: pulse 0.6s ease-in-out;
                }

                @keyframes pulse {
                    0% { transform: scale(1); }
                    50% { transform: scale(1.02); }
                    100% { transform: scale(1); }
                }

                /* Responsive */
                @media (max-width: 768px) {
                    .notification-container {
                        right: 10px;
                        left: 10px;
                        max-width: none;
                    }
                    
                    .notification {
                        margin-bottom: 8px;
                        padding: 12px;
                    }
                }
            `;
            document.head.appendChild(style);
        } catch (error) {
            console.error("Error adding notification styles:", error);
        }
    }

    async connectToHub() {
        try {
            // Kiểm tra xem SignalR đã được load chưa
            if (typeof signalR === 'undefined') {
                console.warn("SignalR not loaded, skipping hub connection");
                return;
            }

            this.connection = new signalR.HubConnectionBuilder()
                .withUrl("/hubThongBao")
                .withAutomaticReconnect()
                .build();

            // Lấy thông tin user từ data attributes
            const userRolesElement = document.querySelector('[data-user-roles]');
            const userIdElement = document.querySelector('[data-user-id]');

            if (userRolesElement) {
                this.userRoles = userRolesElement.getAttribute('data-user-roles').split(',').filter(role => role.trim());
                console.log("User roles:", this.userRoles);
            }

            if (userIdElement) {
                this.userId = userIdElement.getAttribute('data-user-id');
                console.log("User ID:", this.userId);
            }

            // Đăng ký vào các nhóm theo role
            this.connection.on("ReceiveNotification", (notificationData) => {
                console.log("Received notification:", notificationData);
                this.handleNotification(notificationData);
            });

            await this.connection.start();
            console.log("Connected to SignalR Hub");

            // Tham gia các nhóm theo role
            if (this.userRoles.includes("Admin")) {
                await this.connection.invoke("JoinAdminGroup");
                console.log("Joined Admin group");
            }
            if (this.userRoles.includes("Teacher")) {
                await this.connection.invoke("JoinTeacherGroup");
                console.log("Joined Teacher group");
            }
            if (this.userRoles.includes("Student")) {
                await this.connection.invoke("JoinStudentGroup");
                console.log("Joined Student group");
            }
            if (this.userId) {
                await this.connection.invoke("JoinUserGroup", this.userId);
                console.log("Joined User group:", this.userId);
            }

        } catch (error) {
            console.error("SignalR Connection Error:", error);
            // Không throw error để tránh crash toàn bộ hệ thống
        }
    }

    handleNotification(notificationData) {
        try {
            console.log("Raw notification data:", notificationData);
            
            let notification;
            if (typeof notificationData === 'string') {
                try {
                    notification = JSON.parse(notificationData);
                } catch (parseError) {
                    console.error("Error parsing notification JSON:", parseError);
                    // Fallback: treat as simple message
                    notification = {
                        id: Guid().newGuid(),
                        title: 'Thông báo',
                        message: notificationData,
                        type: 'info',
                        timestamp: new Date().toLocaleTimeString('vi-VN')
                    };
                }
            } else {
                notification = notificationData;
            }

            // Cải thiện xử lý thông báo
            if (notification.message && typeof notification.message === 'object') {
                // Nếu message là object, trích xuất thông tin hữu ích
                const messageObj = notification.message;
                if (messageObj.activityName) {
                    notification.message = `Hoạt động: ${messageObj.activityName}`;
                } else if (messageObj.title) {
                    notification.message = messageObj.title;
                } else {
                    notification.message = JSON.stringify(messageObj);
                }
            }

            // Cải thiện title
            if (!notification.title || notification.title === 'Thông báo') {
                switch (notification.type) {
                    case 'success':
                        notification.title = 'Thành công';
                        break;
                    case 'warning':
                        notification.title = 'Cảnh báo';
                        break;
                    case 'error':
                        notification.title = 'Lỗi';
                        break;
                    case 'info':
                    default:
                        notification.title = 'Thông báo';
                        break;
                }
            }

            console.log("Processed notification:", notification);
            this.showNotification(notification);
            this.playNotificationSound(notification.type);
        } catch (error) {
            console.error("Error handling notification:", error);
        }
    }

    showNotification(notification) {
        try {
            if (!this.notificationContainer) {
                console.error("Notification container not found");
                return;
            }

            // Đảm bảo notification.message là string và xử lý đẹp hơn
            let messageString = notification.message;
            if (typeof notification.message === 'object') {
                console.warn("Notification message is object, converting to string:", notification.message);
                messageString = JSON.stringify(notification.message);
            } else if (typeof notification.message !== 'string') {
                messageString = String(notification.message);
            }

            // Xử lý message để hiển thị đẹp hơn
            messageString = this.formatMessage(messageString);

            const notificationElement = document.createElement('div');
            notificationElement.className = `notification ${notification.type}`;
            notificationElement.id = `notification-${notification.id}`;

            const icon = this.getNotificationIcon(notification.type);
            const title = notification.title || 'Thông báo';
            const message = messageString;
            const timestamp = notification.timestamp || new Date().toLocaleTimeString('vi-VN');

            notificationElement.innerHTML = `
                <div class="notification-content">
                    <div class="notification-icon">${icon}</div>
                    <div class="notification-body">
                        <div class="notification-header">
                            <h4 class="notification-title">${title}</h4>
                            <div class="notification-time">${timestamp}</div>
                        </div>
                        <p class="notification-message">${message}</p>
                    </div>
                    <button class="notification-close" onclick="realtimeNotifications.closeNotification('${notification.id}')">
                        <i class="fas fa-times"></i>
                    </button>
                </div>
            `;

            this.notificationContainer.appendChild(notificationElement);

            // Hiển thị animation
            setTimeout(() => {
                notificationElement.classList.add('show');
                notificationElement.classList.add('notification-pulse');
            }, 100);

            // Tự động ẩn sau 8 giây
            setTimeout(() => {
                this.closeNotification(notification.id);
            }, 8000);

            // Giới hạn số lượng thông báo
            this.limitNotifications();
        } catch (error) {
            console.error("Error showing notification:", error);
            // Fallback: hiển thị alert đơn giản
            alert(`${notification.title || 'Thông báo'}: ${messageString || notification.message}`);
        }
    }

    formatMessage(message) {
        try {
            // Loại bỏ các ký tự JSON không cần thiết
            message = message.replace(/[{}"]/g, '');
            
            // Thay thế các key phổ biến
            const replacements = {
                'message:': '',
                'type:': '',
                'title:': '',
                'timestamp:': '',
                'id:': '',
                'Hoạt động mới:': '🆕 Hoạt động mới:',
                'cần xác nhận': '⏳ Cần xác nhận',
                'đã được tạo': '✅ Đã được tạo',
                'đã được cập nhật': '🔄 Đã được cập nhật',
                'đã bị xóa': '🗑️ Đã bị xóa'
            };

            for (const [key, value] of Object.entries(replacements)) {
                message = message.replace(new RegExp(key, 'gi'), value);
            }

            // Loại bỏ các dấu phẩy và khoảng trắng thừa
            message = message.replace(/,\s*/g, ' ').replace(/\s+/g, ' ').trim();

            return message;
        } catch (error) {
            console.error("Error formatting message:", error);
            return message;
        }
    }

    getNotificationIcon(type) {
        switch (type) {
            case 'success':
                return '<i class="fas fa-check-circle" style="color: var(--pixel-green);"></i>';
            case 'warning':
                return '<i class="fas fa-exclamation-triangle" style="color: var(--pixel-yellow);"></i>';
            case 'error':
                return '<i class="fas fa-times-circle" style="color: var(--pixel-red);"></i>';
            case 'info':
            default:
                return '<i class="fas fa-info-circle" style="color: var(--pixel-blue-dark);"></i>';
        }
    }

    closeNotification(notificationId) {
        try {
            const notification = document.getElementById(`notification-${notificationId}`);
            if (notification) {
                notification.classList.remove('notification-pulse');
                notification.classList.add('hide');
                setTimeout(() => {
                    if (notification.parentNode) {
                        notification.parentNode.removeChild(notification);
                    }
                }, 500);
            }
        } catch (error) {
            console.error("Error closing notification:", error);
        }
    }

    limitNotifications() {
        try {
            const notifications = this.notificationContainer.querySelectorAll('.notification');
            if (notifications.length > this.maxNotifications) {
                const oldestNotification = notifications[0];
                this.closeNotification(oldestNotification.id.replace('notification-', ''));
            }
        } catch (error) {
            console.error("Error limiting notifications:", error);
        }
    }

    playNotificationSound(type) {
        try {
            const audioContext = new (window.AudioContext || window.webkitAudioContext)();
            const oscillator = audioContext.createOscillator();
            const gainNode = audioContext.createGain();
            
            oscillator.connect(gainNode);
            gainNode.connect(audioContext.destination);
            
            // Tần số khác nhau cho từng loại thông báo
            let frequency = 440; // Mặc định
            switch (type) {
                case 'success':
                    frequency = 800; // Cao
                    break;
                case 'warning':
                    frequency = 600; // Trung bình cao
                    break;
                case 'error':
                    frequency = 200; // Thấp
                    break;
                case 'info':
                    frequency = 440; // Trung bình
                    break;
            }
            
            oscillator.frequency.setValueAtTime(frequency, audioContext.currentTime);
            oscillator.type = 'sine';
            
            gainNode.gain.setValueAtTime(0.1, audioContext.currentTime);
            gainNode.gain.exponentialRampToValueAtTime(0.001, audioContext.currentTime + 0.1);
            
            oscillator.start(audioContext.currentTime);
            oscillator.stop(audioContext.currentTime + 0.1);
        } catch (error) {
            console.log("Audio not supported or error:", error);
        }
    }

    setupEventListeners() {
        try {
            // Thêm event listener cho việc đóng tất cả thông báo
            document.addEventListener('keydown', (e) => {
                if (e.key === 'Escape') {
                    this.closeAllNotifications();
                }
            });
        } catch (error) {
            console.error("Error setting up event listeners:", error);
        }
    }

    closeAllNotifications() {
        try {
            const notifications = this.notificationContainer.querySelectorAll('.notification');
            notifications.forEach(notification => {
                const id = notification.id.replace('notification-', '');
                this.closeNotification(id);
            });
        } catch (error) {
            console.error("Error closing all notifications:", error);
        }
    }
}

// Khởi tạo hệ thống thông báo khi trang load
let realtimeNotifications;

// Hàm khởi tạo an toàn
function initializeRealtimeNotifications() {
    try {
        if (!realtimeNotifications) {
            realtimeNotifications = new RealtimeNotifications();
            // Expose ra window object để các file khác có thể truy cập
            window.realtimeNotifications = realtimeNotifications;
            console.log("RealtimeNotifications initialized successfully");
        }
    } catch (error) {
        console.error("Error initializing RealtimeNotifications:", error);
    }
}

// Khởi tạo ngay lập tức nếu DOM đã sẵn sàng
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', function() {
        initializeRealtimeNotifications();
    });
} else {
    // DOM đã sẵn sàng, khởi tạo ngay
    initializeRealtimeNotifications();
}

// Utility function để tạo GUID
function Guid() {
    return {
        newGuid: function() {
            return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
                var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
                return v.toString(16);
            });
        }
    };
} 