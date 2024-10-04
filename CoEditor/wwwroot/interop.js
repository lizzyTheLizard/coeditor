window.getSelectionStart = function (textareaId) {
    var textarea = document.getElementById(textareaId);
    if (textarea) {
        return textarea.selectionStart
    }
    return -1;
};

window.getSelectionEnd = function (textareaId) {
    var textarea = document.getElementById(textareaId);
    if (textarea) {
        return textarea.selectionEnd
    }
    return -1;
};

