window.msSaveBlob = function (payload, filename) {

    const createBlob = data => new Blob([data], { type: "text/csv;charset=utf-8;" });

    const buildDownloadLink = (blob, fileName) => {
        let link = document.createElement("a");
        link.setAttribute("href", URL.createObjectURL(blob));
        link.setAttribute("download", fileName);
        link.style = "visibility:hidden";
        return link;
    }
    const invokeDownload = link => {
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }
    const isHtmlDownloadAllowed = document.createElement("a").download !== undefined;
    const isSaveBlobAllowed = navigator.msSaveBlob;

    isSaveBlobAllowed ? navigator.msSaveBlob(createBlob(payload), filename) :
        isHtmlDownloadAllowed ? invokeDownload(buildDownloadLink(createBlob(payload), filename)) :
            console.log("Feature unsupported");

};