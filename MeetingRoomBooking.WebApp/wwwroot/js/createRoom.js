function toggleClass(checkbox, labelId) {
    const label = document.getElementById(labelId); // Get the label element by ID
    if (checkbox.checked) {
        label.classList.add('toggle-active'); // Add active class if checked
    } else {
        label.classList.remove('toggle-active'); // Remove active class if unchecked
    }
}

function validateImage() {
	const fileInput = document.getElementById('imageUpload');
	const filePath = fileInput.value;
	const allowedExtensions = /(\.jpg|\.jpeg|\.png)$/i;

	if (!allowedExtensions.exec(filePath)) {
		alert('Please upload an image file (jpg, jpeg, png).');
		fileInput.value = ''; // Clear the input
		return false;
	}
}
const dropArea = document.getElementById('dropArea');
const inputFile = document.getElementById('imageUpload');
const previewImage = document.getElementById('previewImage');
const uploadText = document.getElementById('uploadText');

dropArea.addEventListener('dragover', (e) => {
	e.preventDefault();
	dropArea.classList.add('dragover');
});

dropArea.addEventListener('dragleave', () => {
	dropArea.classList.remove('dragover');
});

dropArea.addEventListener('drop', (e) => {
	e.preventDefault();
	dropArea.classList.remove('dragover');

	const files = e.dataTransfer.files;
	if (files.length > 0) {
		inputFile.files = files;
		displayImage();
	}
});

dropArea.addEventListener('click', () => {
	inputFile.click();
});

function displayImage() {
	const file = inputFile.files[0];
	if (file && file.type.startsWith('image/')) {
		const reader = new FileReader();
		reader.onload = function (e) {
			previewImage.src = e.target.result;
			previewImage.style.display = 'block';
			uploadText.style.display = 'none';
		};
		reader.readAsDataURL(file);
	} else {
		console.error('Please select a valid image.');
	}
}