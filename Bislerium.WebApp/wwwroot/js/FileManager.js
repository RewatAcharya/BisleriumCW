function compressAndValidate(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            var img = new Image();
            img.src = e.target.result;

            img.onload = function () {
                var canvas = document.createElement('canvas');
                var ctx = canvas.getContext('2d');

                // Calculate the new width and height to maintain the aspect ratio
                var MAX_WIDTH = 800;
                var MAX_HEIGHT = 600;
                var width = img.width;
                var height = img.height;
                if (width > height) {
                    if (width > MAX_WIDTH) {
                        height *= MAX_WIDTH / width;
                        width = MAX_WIDTH;
                    }
                } else {
                    if (height > MAX_HEIGHT) {
                        width *= MAX_HEIGHT / height;
                        height = MAX_HEIGHT;
                    }
                }

                canvas.width = width;
                canvas.height = height;
                ctx.drawImage(img, 0, 0, width, height);

                // Convert the canvas image to Blob
                canvas.toBlob(function (blob) {
                    if (blob.size > 3 * 1024 * 1024) {
                        // Image size is smaller than 3 MB after compression
                        alert("The compressed image size is greater than 3 MB. Please choose a different image.");
                        input.value = ''; // Clear the file input
                    } else {
                        // Image size is 3 MB or larger, proceed with upload
                        console.log("Compressed image size:", blob.size);
                    }
                }, input.files[0].type);
            };
        };

        reader.readAsDataURL(input.files[0]);
    }
}