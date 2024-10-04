window.getSelectedText = function (textareaId) {
    var textarea = document.getElementById(textareaId);
    if (textarea) {
        return textarea.value.substring(textarea.selectionStart, textarea.selectionEnd);
    }
    return '';
};
