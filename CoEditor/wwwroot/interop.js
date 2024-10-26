window.showModal = function (modalId) {
    const myModal = new bootstrap.Modal(document.getElementById(modalId));
    myModal.show();
}

window.getSelectionStart = function (textareaId) {
    const textarea = document.getElementById(textareaId);
    if (textarea) {
        return textarea.selectionStart
    }
    return -1;
};

window.getSelectionEnd = function (textareaId) {
    const textarea = document.getElementById(textareaId);
    if (textarea) {
        return textarea.selectionEnd
    }
    return -1;
};

window.addKeyboardListener = function (dotNetHelper) {
    window.document.addEventListener('keydown', async function (e) {
        if (!e) return;
        let cSharpKeyboardEventArgs = {
            key: e.key,
            code: e.code,
            location: e.location,
            repeat: e.repeat,
            ctrlKey: e.ctrlKey,
            shiftKey: e.shiftKey,
            altKey: e.altKey,
            metaKey: e.metaKey,
            type: e.type
        };
        await dotNetHelper.invokeMethodAsync('HandleKeyboardEventAsync', cSharpKeyboardEventArgs);
    });
};

