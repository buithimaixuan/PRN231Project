let cropper;

function loadImage(event) {
    const input = event.target;
    if (input.files && input.files[0]) {
        const reader = new FileReader();
        reader.onload = function (e) {
            const image = document.getElementById('image');
            image.src = e.target.result;
            image.style.display = 'block';

            // Khởi tạo cropper
            if (cropper) {
                cropper.destroy(); // Hủy cropper cũ nếu đã khởi tạo
            }
            cropper = new Cropper(image, {
                aspectRatio: 1, // Tỉ lệ hình vuông
                viewMode: 1,    // Giới hạn ảnh trong khung nhìn
                autoCropArea: 1, // Diện tích cắt ban đầu chiếm 80% ảnh
                responsive: true,
                modal: true,    // Hiển thị lớp phủ mờ
                guides: true,   // Hiển thị đường lưới
                center: true,   // Hiển thị tâm cắt
                highlight: true,// Làm nổi bật vùng cắt
                background: true,// Hiển thị nền lưới
                dragMode: 'move', // Cho phép di chuyển ảnh bằng cách kéo
                scalable: true,   // Cho phép phóng to/thu nhỏ
                zoomable: true,   // Cho phép zoom bằng bánh xe chuột hoặc chạm
                zoomOnTouch: true,// Zoom bằng hai ngón tay trên mobile
                zoomOnWheel: true,// Zoom bằng bánh xe chuột trên PC
                movable: true,    // Cho phép di chuyển ảnh
                cropBoxMovable: false, // Không cho di chuyển khung cắt
                cropBoxResizable: false, // Không cho thay đổi kích thước khung cắt
                crop: function (event) {
                    // Xử lý khi cắt ảnh nếu cần
                },
            });
        }
        reader.readAsDataURL(input.files[0]);
    }
}

document.getElementById('cropButton').addEventListener('click', function () {
    if (cropper) {
        const canvas = cropper.getCroppedCanvas({
            width: 300,
            height: 300,
        });

        canvas.toBlob(function (blob) {
            const file = new File([blob], "croppedImage.png", { type: "image/png" });
            const dataTransfer = new DataTransfer();
            dataTransfer.items.add(file);
            document.getElementById('fileInput').files = dataTransfer.files; // Gán file đã cắt vào input file
            $('#avatarModal').modal('hide'); // Đóng modal sau khi cắt
            // Thực hiện submit form ở đây nếu cần thiết
            document.getElementById('avatarForm').submit(); // Tự động submit form
        });
    }
});