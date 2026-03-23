# 🚀 Hướng Dẫn Sử Dụng Hệ Thống Realtime

## 📋 Tổng Quan

Hệ thống đã được cập nhật để tất cả các chức năng đều hoạt động realtime mà không cần reload trang!

## ✨ Các Tính Năng Realtime

### 1. **Tạo Hoạt Động Realtime**
- ✅ Form tạo hoạt động sử dụng AJAX
- ✅ Thông báo thành công/lỗi realtime
- ✅ Cập nhật danh sách hoạt động tự động
- ✅ Gửi thông báo cho Admin ngay lập tức

### 2. **Đăng Ký Tham Gia Realtime**
- ✅ Form đăng ký sử dụng AJAX
- ✅ Cập nhật trạng thái đăng ký realtime
- ✅ Thông báo cho Teacher và Admin
- ✅ Cập nhật số lượng đăng ký tự động

### 3. **Xác Nhận Đăng Ký Realtime**
- ✅ Form xác nhận sử dụng AJAX
- ✅ Cập nhật trạng thái realtime
- ✅ Thông báo cho Student ngay lập tức
- ✅ Cập nhật danh sách đăng ký tự động

### 4. **Xác Nhận Hoạt Động Realtime**
- ✅ Form xác nhận hoạt động sử dụng AJAX
- ✅ Thông báo cho tất cả user
- ✅ Cập nhật trạng thái hoạt động realtime

### 5. **Thống Kê Realtime**
- ✅ Cập nhật số liệu thống kê tự động (30 giây)
- ✅ Cập nhật danh sách hoạt động (60 giây)
- ✅ Hiển thị indicator khi có cập nhật

### 6. **Thông Báo Realtime**
- ✅ Popup thông báo realtime
- ✅ Lưu thông báo vào database
- ✅ Hiển thị trong "Thông báo của tôi"
- ✅ Âm thanh và animation đẹp mắt

### 7. **Hệ Thống Gợi Ý Bằng AI (Content-Based Recommender System)**
- ✅ Xây dựng bằng Python FastAPI & scikit-learn
- ✅ Thuật toán TF-IDF kết hợp Cosine Similarity phân tích ngôn ngữ tự nhiên
- ✅ Tự động tính toán độ tương đồng nội dung giữa hoạt động hiện tại và các hoạt động khác
- ✅ Gợi ý thông minh Top 4 hoạt động liên quan ngay bên dưới trang chi tiết sự kiện
- ✅ Giao tiếp liên dịch vụ (Microservices architecture) giữa ASP.NET Core backend và Python AI service

## 🎯 Cách Sử Dụng

### **Cho Admin:**
1. **Tạo hoạt động**: Form sẽ submit realtime, không reload trang
2. **Xác nhận hoạt động**: Click xác nhận → cập nhật ngay lập tức
3. **Xem thống kê**: Số liệu tự động cập nhật mỗi 30 giây
4. **Thông báo**: Nhận popup realtime khi có sự kiện mới

### **Cho Teacher:**
1. **Tạo hoạt động**: Form submit realtime
2. **Xác nhận đăng ký**: Click xác nhận → cập nhật ngay
3. **Nhận thông báo**: Popup khi có đăng ký mới
4. **Xem thống kê**: Cập nhật tự động

### **Cho Student:**
1. **Đăng ký tham gia**: Form submit realtime
2. **Nhận thông báo**: Popup khi đăng ký được xác nhận
3. **Xem trạng thái**: Cập nhật realtime
4. **Hủy đăng ký**: Thao tác realtime

## 🔧 Các File Đã Cập Nhật

### **Backend (Controllers):**
- `HoatDongController.cs` - Hỗ trợ AJAX cho tạo/sửa/xác nhận
- `DangKyThamGiaController.cs` - Hỗ trợ AJAX cho đăng ký/xác nhận
- `AdminController.cs` - Hỗ trợ AJAX cho thống kê

### **Frontend (JavaScript):**
- `realtime-functions.js` - Xử lý tất cả chức năng realtime
- `realtime-notifications.js` - Hệ thống thông báo realtime

### **CSS:**
- `realtime-styles.css` - Styles cho realtime effects

### **Views:**
- `_ActivityList.cshtml` - Partial view cho danh sách hoạt động
- `_Layout.cshtml` - Thêm scripts và CSS realtime
- `Details.cshtml` - Thêm phần UI hiển thị danh sách hoạt động gợi ý

### **Python Microservice (AI Recommender):**
- Thư mục `python_recommender/`: Chứa file `main.py` (FastAPI) chuyên xử lý text (Tokenization, TF-IDF Vectorization, Cosine Similarity) và `requirements.txt`.
- `Services/RecommenderService.cs` - Tích hợp `HttpClient` gọi nội bộ sang Python API lấy IDs độ chính xác cao.

## 🚀 Hướng Dẫn Chạy Cụm Service AI (Recommender System)
Để tiện ích gợi ý hoạt động dựa trên NLP chạy được, trước khi `dotnet run`, bạn cần bật API Python độc lập lên:
1. Mở terminal, đi vào thư mục engine: `cd python_recommender`
2. Cài đặt các thư viện lõi: `py -m pip install -r requirements.txt` (hoặc `python -m pip...`)
3. Bật FastAPI Server độc lập: `py -m uvicorn main:app --port 8000`
4. Mở tab terminal khác ở thư mục gốc và chạy `dotnet run` như bình thường.

## 🎨 Hiệu Ứng Realtime

### **Loading States:**
- Button hiển thị spinner khi đang xử lý
- Form bị disable khi submit
- Animation loading đẹp mắt

### **Success/Error Feedback:**
- Thông báo popup realtime
- Alert tự động ẩn sau 5 giây
- Màu sắc phân biệt thành công/lỗi

### **Auto-refresh:**
- Thống kê cập nhật mỗi 30 giây
- Danh sách cập nhật mỗi 60 giây
- Indicator hiển thị khi có cập nhật

### **Animations:**
- Slide in cho item mới
- Highlight cho item được cập nhật
- Pulse cho status badge
- Fade in/out cho notifications

## 🔄 Luồng Hoạt Động

### **Tạo Hoạt Động:**
1. User điền form → Submit
2. AJAX gửi request → Server xử lý
3. Lưu vào database → Gửi thông báo realtime
4. Cập nhật UI → Hiển thị thông báo thành công
5. Reload danh sách hoạt động

### **Đăng Ký Tham Gia:**
1. Student đăng ký → AJAX submit
2. Server xử lý → Lưu database
3. Gửi thông báo cho Teacher/Admin
4. Cập nhật trạng thái realtime
5. Hiển thị thông báo thành công

### **Xác Nhận Đăng Ký:**
1. Teacher/Admin xác nhận → AJAX submit
2. Server cập nhật trạng thái
3. Gửi thông báo cho Student
4. Cập nhật UI realtime
5. Hiển thị thông báo thành công

## 🚨 Lưu Ý Quan Trọng

### **Browser Support:**
- ✅ Chrome, Firefox, Safari, Edge
- ✅ Mobile browsers
- ✅ Responsive design

### **Network Issues:**
- Fallback cho mất kết nối
- Retry mechanism
- Offline indicator

### **Performance:**
- Debounced updates
- Efficient DOM updates
- Memory leak prevention

## 🎉 Kết Quả

Bây giờ hệ thống hoạt động hoàn toàn realtime:

✅ **Không cần reload trang** cho bất kỳ thao tác nào  
✅ **Thông báo realtime** cho tất cả sự kiện  
✅ **Cập nhật UI tự động** khi có thay đổi  
✅ **Trải nghiệm mượt mà** như các ứng dụng hiện đại  
✅ **Hiệu ứng đẹp mắt** với animation và âm thanh  

Hệ thống giờ đây hoạt động như một Single Page Application (SPA) với tất cả tính năng realtime! 🚀 
