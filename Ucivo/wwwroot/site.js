// Záchranný fallback: --training-vh nastavíme ihned synchronně
// (JS init přijde až po Blazor OnAfterRenderAsync – tato hodnota mezitím drží layout)
;(function () {
    var vh = window.visualViewport ? window.visualViewport.height : window.innerHeight;
    document.documentElement.style.setProperty('--training-vh', vh + 'px');
})();

// Training layout - prizpusobeni vysky pri zobrazeni klavesnice + blokace scrollu
window.trainingLayout = {
    _vpHandler: null,
    _windowScrollHandler: null,
    _touchMoveHandler: null,

    init: function () {
        // Přesně zarovnat kontejner na visualViewport
        // – funguje pro Android (resize) i iOS (scroll při klávesnici)
        const positionContainer = () => {
            const vp = window.visualViewport;
            const el = document.querySelector('.training-session-container');
            if (!el) return;

            if (vp) {
                el.style.top    = Math.round(vp.offsetTop)  + 'px';
                el.style.left   = Math.round(vp.offsetLeft) + 'px';
                el.style.width  = Math.round(vp.width)      + 'px';
                el.style.height = Math.round(vp.height)     + 'px';
            } else {
                el.style.top    = '0';
                el.style.left   = '0';
                el.style.width  = '100%';
                el.style.height = window.innerHeight + 'px';
            }
        };

        this._vpHandler = positionContainer;
        positionContainer(); // okamžité nastavení před prvním fokusem

        if (window.visualViewport) {
            window.visualViewport.addEventListener('resize', this._vpHandler);
            window.visualViewport.addEventListener('scroll', this._vpHandler);
        } else {
            window.addEventListener('resize', this._vpHandler);
        }

        // Zabránit window scroll (iOS odskroluje při otevření klávesnice)
        this._windowScrollHandler = () => {
            if (window.scrollY > 0) window.scrollTo(0, 0);
        };
        window.addEventListener('scroll', this._windowScrollHandler, { passive: true });

        // Zabránit pull-to-refresh a bounce efektu na iOS/Android
        this._touchMoveHandler = (e) => {
            if (e.target.tagName === 'INPUT' || e.target.tagName === 'TEXTAREA') return;
            e.preventDefault();
        };
        document.body.addEventListener('touchmove', this._touchMoveHandler, { passive: false });
    },

    destroy: function () {
        if (this._touchMoveHandler) {
            document.body.removeEventListener('touchmove', this._touchMoveHandler);
            this._touchMoveHandler = null;
        }
        if (window.visualViewport && this._vpHandler) {
            window.visualViewport.removeEventListener('resize', this._vpHandler);
            window.visualViewport.removeEventListener('scroll', this._vpHandler);
        } else if (this._vpHandler) {
            window.removeEventListener('resize', this._vpHandler);
        }
        if (this._windowScrollHandler) {
            window.removeEventListener('scroll', this._windowScrollHandler);
            this._windowScrollHandler = null;
        }
        this._vpHandler = null;

        // Resetuj přímé styly kontejneru
        const el = document.querySelector('.training-session-container');
        if (el) {
            el.style.top = el.style.left = el.style.width = el.style.height = '';
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

