// Dropdown toggle s debouncing - pøipojit event listener
let dropdownToggling = false;
let dotNetHelperInstance = null;
let eventListenerAdded = false; // Pøíznak aby se listener pøidal jen jednou

window.initializeDropdown = function(dotNetHelper) {
    dotNetHelperInstance = dotNetHelper;
    console.log("[JS] initializeDropdown called");
    
    // Pøidat listener jen jednou!
    if (eventListenerAdded) {
        console.log("[JS] Event listener already exists, skipping");
        return;
    }
    
    const btn = document.getElementById('user-menu-btn');
    if (btn) {
        btn.addEventListener('click', function(e) {
            e.preventDefault();
            e.stopPropagation();
            
            if (dropdownToggling) {
                console.log("[JS] Click IGNORED (debouncing)");
                return;
            }
            
            dropdownToggling = true;
            console.log("[JS] Click captured, calling Blazor");
            
            if (dotNetHelperInstance) {
                dotNetHelperInstance.invokeMethodAsync('ToggleDropdownFromJS');
            }
            
            setTimeout(() => {
                dropdownToggling = false;
            }, 300);
        });
        eventListenerAdded = true;
        console.log("[JS] Event listener added (first time only)");
    }
};

