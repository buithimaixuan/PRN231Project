

//document.addEventListener('DOMContentLoaded', () => {
//    const btnUpdatePost = document.querySelectorAll('.btnUpdatePost');
//    const popupUpdate = document.querySelector('#popupIdUpdatePost');
//    const closePopupBtn = document.getElementById('closePopupUpdateBtn');

//    console.log(btnUpdatePost);
//    // Đảm bảo popup bị ẩn khi khởi động
//    popupUpdate.style.display = 'none';


//    //MAPPING DỮ LIỆU LÊN FORM
//    // Lặp qua tất cả các nút "Chỉnh sửa"
//    const idUpdatePost = document.getElementById('idUpdatePost');
//    const postContentField = popupUpdate.querySelector('textarea[name="ContentPost"]');
//    const imageUpdatePreview = document.getElementById('imageUpdatePreview');
//    const categorySelect = document.getElementById('select-input-update');

//    //Xử lý khi nhấn nút open modal
//    btnUpdatePost.forEach(button => {
//        button.addEventListener('click', () => {
//            // Lấy dữ liệu từ các thuộc tính data-*
//            const postId = button.getAttribute('data-post-id');
//            const postContent = button.getAttribute('data-post-content');
//            const postImages = JSON.parse(button.getAttribute('data-post-images'));
//            const postCategory = button.getAttribute('data-post-category');

//            // Gán giá trị vào các trường trong modal
//            idUpdatePost.value = postId;
//            postContentField.value = postContent;

//            if (postCategory) {
//                categorySelect.value = postCategory;
//            }

//            // Xóa ảnh cũ trong `imageUpdatePreview`
//            imageUpdatePreview.innerHTML = '';
//            postImages.forEach(imageUrl => {
//                const imgElement = document.createElement('img');
//                imgElement.src = imageUrl;
//                imgElement.style.width = '100px'; // Kích thước hình ảnh
//                imgElement.style.margin = '5px';
//                imageUpdatePreview.appendChild(imgElement);
//            });

//            // Hiển thị popup và thiết lập overlay
//            openModal();

//            console.log('Hello');
//        });
//    });


//    //Xử lý nút close modal
//    closePopupBtn.addEventListener('click', () => {
//        closeModal();
//    });

//    function openModal() {
//        console.log('hello modal');
//        popupUpdate.style.display = 'flex';
//        popupUpdate.style.zIndex = '1050';

//            // Đảm bảo chỉ có một overlay
//            let modalBackdrop = document.querySelector('.modal-backdrop');
//            if (!modalBackdrop) {
//                modalBackdrop = document.createElement('div');
//                modalBackdrop.className = 'modal-backdrop fade show';
//                modalBackdrop.style.zIndex = '1040';
//                document.body.appendChild(modalBackdrop);
//            }

//            body.classList.add('modal-open'); // Khóa cuộn trang
//    }

//    function closeModal() {
//        popupUpdate.style.display = 'none';
//        popupUpdate.style.zIndex = '0';

//        // Xóa overlay nếu có
//        const modalBackdrop = document.querySelector('.modal-backdrop');
//        if (modalBackdrop) {
//            modalBackdrop.remove();
//        }

//        document.body.classList.remove('modal-open'); // Mở khóa cuộn trang
//    }

//});





