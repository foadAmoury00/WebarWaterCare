mergeInto(LibraryManager.library, {
  DownloadDataUrl: function (filenamePtr, dataUrlPtr) {
    const filename = UTF8ToString(filenamePtr);
    const dataUrl  = UTF8ToString(dataUrlPtr);

    const a = document.createElement('a');
    a.href = dataUrl;
    a.download = filename || 'screenshot.png';
    document.body.appendChild(a);
    a.click();
    a.remove();
  }
});
