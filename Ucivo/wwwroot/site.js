// Training layout - prizpusobeni vysky pri zobrazeni klavesnice + blokace scrollu
window.trainingLayout = {
    _touchMoveHandler: null,
    _viewportResizeHandler: null,

    init: function () {
        this._viewportResizeHandler = () => {
            const vh = window.visualViewport ? window.visualViewport.height : window.innerHeight;
            document.documentElement.style.setProperty('--training-vh', vh + 'px');
        };
        this._viewportResizeHandler();

        if (window.visualViewport) {
            window.visualViewport.addEventListener('resize', this._viewportResizeHandler);
        } else {
            window.addEventListener('resize', this._viewportResizeHandler);
        }

        // Zabranit pull-to-refresh a bounce efektu na iOS/Android
        this._touchMoveHandler = function (e) { e.preventDefault(); };
        document.body.addEventListener('touchmove', this._touchMoveHandler, { passive: false });
    },

    destroy: function () {
        // Odstranit listener pri opusteni stranky - jinak by blokoval scroll vsude!
        if (this._touchMoveHandler) {
            document.body.removeEventListener('touchmove', this._touchMoveHandler);
            this._touchMoveHandler = null;
        }
        if (this._viewportResizeHandler) {
            if (window.visualViewport) {
                window.visualViewport.removeEventListener('resize', this._viewportResizeHandler);
            } else {
                window.removeEventListener('resize', this._viewportResizeHandler);
            }
            this._viewportResizeHandler = null;
        }
    }
};

// Dropdown toggle s debouncing - p�ipojit event listener
let dropdownToggling = false;
let dotNetHelperInstance = null;
let eventListenerAdded = false; // P��znak aby se listener p�idal jen jednou

window.initializeDropdown = function(dotNetHelper) {
    dotNetHelperInstance = dotNetHelper;
    console.log("[JS] initializeDropdown called");
    
    // P�idat listener jen jednou!
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

