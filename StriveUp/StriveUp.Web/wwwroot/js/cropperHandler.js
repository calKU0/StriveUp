
window.cropImage = async (imageDataUrl) => {
    // Create an image object
    const img = new Image();
img.src = imageDataUrl;

    return new Promise((resolve) => {
    img.onload = () => {
        const canvas = document.createElement('canvas');
        const ctx = canvas.getContext('2d');

        // Set canvas size to 256x256 (cropping size)
        const size = 256;
        canvas.width = size;
        canvas.height = size;

        // Draw the image to the canvas and crop it
        ctx.drawImage(img, 0, 0, size, size);

        // Get the cropped image as a base64 string
        const croppedBase64 = canvas.toDataURL();
        resolve(croppedBase64);
    };
    });
};

