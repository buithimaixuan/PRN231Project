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
                viewMode: 1,
                autoCropArea: 1,
                responsive: true,
                modal: true,
                guides: true,
                center: true,
                highlight: true,
                background: true,
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
            width: 110,
            height: 110,
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