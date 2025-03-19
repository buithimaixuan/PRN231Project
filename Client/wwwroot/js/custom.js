function expandSearch() {
    var headSearch = document.querySelector('.head-search');
    var searchSuggestions = document.getElementById('searchSuggestions');
    var searchContainer = document.querySelector('.search-container');

    headSearch.classList.add('expand');
    searchSuggestions.classList.add('showSuggest');
    searchContainer.classList.add('showBoxShadow');
}

function collapseSearch() {
    var headSearch = document.querySelector('.head-search');
    var searchSuggestions = document.getElementById('searchSuggestions');
    var searchContainer = document.querySelector('.search-container');

    headSearch.classList.remove('expand');
    searchSuggestions.classList.remove('showSuggest');
    searchContainer.classList.remove('showBoxShadow');
}

function exitSearch() {
    var searchInput = document.getElementById('searchInput');
    searchInput.blur();  // Remove focus from the search input
}

//POP UP ADD POST 
const imageInput = document.getElementById('imageInput');
const imagePreview = document.getElementById('imagePreview');
const clearImagesButton = document.getElementById('clearImagesButton');

// Mảng để lưu các file đã chọn
let selectedFiles = [];

// Sự kiện cho nút chọn ảnh
document.getElementById('selectImageButton').addEventListener('click', () => {
    imageInput.click();
});

// Sự kiện cho input file
imageInput.addEventListener('change', (event) => {
    imagePreview.innerHTML = '';
    const files = Array.from(event.target.files);

    files.forEach(file => {
        //selectedFiles.push(file);
        const reader = new FileReader();
        reader.onload = function (e) {
            const img = document.createElement('img');
            img.src = e.target.result;
            img.style.width = '100px'; // Kích thước hình ảnh
            img.style.marginRight = '10px';
            imagePreview.appendChild(img);
        };
        reader.readAsDataURL(file);
    });
    //clearImagesButton.style.display = selectedFiles.length > 0 ? 'block' : 'none';
});

// Sự kiện cho nút xóa ảnh
clearImagesButton.addEventListener('click', () => {
    imagePreview.innerHTML = ''; // Xóa ảnh
    selectedFiles = []; // Xóa danh sách file đã chọn
    //clearImagesButton.style.display = 'none'; // Ẩn nút xóa ảnh
});

// Sự kiện cho nút submit
//document.getElementById('postForm').addEventListener('submit', function (event) {
//    // Tạo một DataTransfer để thêm file vào input file
//    const dataTransfer = new DataTransfer();
//    selectedFiles.forEach(file => {
//        dataTransfer.items.add(file);
//    });
//    imageInput.files = dataTransfer.files; // Cập nhật giá trị của input file
//});


document.addEventListener('DOMContentLoaded', () => {
    const postInput = document.getElementById('postInput');
    const popup = document.getElementById('popup');
    const closePopupBtn = document.getElementById('closePopupBtn');

    // Đảm bảo popup bị ẩn khi khởi động
    popup.style.display = 'none';

    postInput.addEventListener('click', () => {

        popup.style.display = 'flex';
        popup.style.zIndex = '1050';

        // Đảm bảo chỉ có một overlay
        let modalBackdrop = document.querySelector('.modal-backdrop');
        if (!modalBackdrop) {
            modalBackdrop = document.createElement('div');
            modalBackdrop.className = 'modal-backdrop fade show';
            modalBackdrop.style.zIndex = '1040';
            document.body.appendChild(modalBackdrop);
        }

        imagePreview.innerHTML = '';
        body.classList.add('modal-open'); // Khóa cuộn trang
    });

    closePopupBtn.addEventListener('click', () => {
        popup.style.display = 'none';
        popup.style.zIndex = '0';

        // Xóa overlay nếu có
        const modalBackdrop = document.querySelector('.modal-backdrop');
        if (modalBackdrop) {
            modalBackdrop.remove();
        }

        document.body.classList.remove('modal-open'); // Mở khóa cuộn trang
    });
});


