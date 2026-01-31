// CSX Bootloader
// 1. Listens for events on document root.
// 2. Extracts handler IDs.
// 3. Dispatches to Wasm (loading it if necessary).

(function() {
    window.CSX = {
        dispatch: function(id) {
            console.log("Dispatching to Wasm:", id);
            // In a real app, this would call:
            // exports.Program.Dispatch(id);
        }
    };

    document.addEventListener('click', function(e) {
        let target = e.target;
        while (target && target !== document) {
            const handlerId = target.getAttribute('onclick-csx');
            if (handlerId) {
                window.CSX.dispatch(handlerId);
                // Prevent default mechanism if needed, but usually we let the Wasm handler decide
            }
            target = target.parentNode;
        }
    });

    console.log("CSX Bootloader Initialized");
})();
