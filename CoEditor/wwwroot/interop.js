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

window.setFocus = function (inputId) {
    const input = document.getElementById(inputId);
    if (input) {
        console.log('Set focus to ' + inputId);
        window.setTimeout(() => input.focus(), 0);
    }
};

window.addKeyboardListener = function (dotNetHelper) {
    window.document.addEventListener('keydown', async function (e) {
        if (!e) return;
        if(!e.altKey) return;
        if(e.ctrlKey) return;
        let handled = await dotNetHelper.invokeMethodAsync('HandleKeyboardEventAsync', e.key[0]);
        if(handled) e.preventDefault();
    });
};

