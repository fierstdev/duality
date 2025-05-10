// File: duality/src/components/base_component.ts (Conceptual)
// This file would define the core DualityComponent base class.

/**
 * DualityComponent - Base class for all Duality Web Components.
 *
 * This class provides the foundational structure and lifecycle integration
 * for components built with the Duality framework. It will handle:
 * - Shadow DOM creation (defaulting to open).
 * - Template rendering.
 * - Style encapsulation.
 * - Integration with Duality's reactivity system (in later PoCs).
 * - Lifecycle hooks.
 */
export class DualityComponent extends HTMLElement {
	protected _shadowRoot: ShadowRoot;
	private _templateFn: (() => string) | null = null; // Function that returns HTML string
	private _styles: CSSStyleSheet[] = [];          // For adopted stylesheets (native styles)

	constructor() {
		super();
		// Create an open shadow root by default.
		// Framework users could potentially configure this (open/closed).
		this._shadowRoot = this.attachShadow({ mode: 'open' });
	}

	/**
	 * Native connectedCallback, called when the element is added to the DOM.
	 * Duality will provide its own lifecycle hooks like `onConnected` that
	 * developers will typically use instead of directly overriding this.
	 */
	connectedCallback() {
		this._render();
		// deno-lint-ignore no-explicit-any
		if (typeof (this as any).onConnected === 'function') {
			// deno-lint-ignore no-explicit-any
			(this as any).onConnected();
		}
	}

	/**
	 * Native disconnectedCallback.
	 */
	disconnectedCallback() {
		// deno-lint-ignore no-explicit-any
		if (typeof (this as any).onDisconnected === 'function') {
			// deno-lint-ignore no-explicit-any
			(this as any).onDisconnected();
		}
	}

	/**
	 * Internal render method.
	 * This will be expanded significantly to handle reactivity, template parsing, etc.
	 * For now, it just applies the template and styles.
	 */
	protected _render() {
		if (this._styles.length > 0) {
			this._shadowRoot.adoptedStyleSheets = [...this._styles];
		}

		if (this._templateFn) {
			// In a real scenario, this would involve a more sophisticated template engine
			// that can handle data bindings, conditionals, loops, and event listeners.
			// For this PoC, it's a simple string.
			this._shadowRoot.innerHTML = this._templateFn();
		} else {
			// Fallback or error if no template is defined for a component that expects one.
			this._shadowRoot.innerHTML = '<p>No template defined for this component.</p>';
		}
	}

	/**
	 * Method to be called by decorators to set the template function.
	 * @internal
	 */
	_setTemplate(templateFn: () => string) {
		this._templateFn = templateFn;
	}

	/**
	 * Method to be called by decorators to add native CSS styles.
	 * @internal
	 */
	_addStyles(stylesheet: CSSStyleSheet) {
		this._styles.push(stylesheet);
	}

	// Placeholder for future lifecycle hooks Duality will provide
	// onConnected?(): void;
	// onDisconnected?(): void;
	// onPropsChanged?(changedProps: Map<string, any>): void;
	// beforeUpdate?(): void;
	// afterUpdate?(): void;
}

// File: duality/src/components/decorators.ts (Conceptual)
// This file would define decorators used with DualityComponent.
export function CustomElement(tagName: string) {
	return function (constructor: CustomElementConstructor) {
		if (customElements.get(tagName)) {
			console.warn(`Custom element "${tagName}" is already defined. Skipping redefinition.`);
		} else {
			customElements.define(tagName, constructor);
		}
	};
}

/**
 * @Template decorat
 * Associates an HTML template string with a DualityComponent.
 * The template string can later support bindings.
 * For this PoC, it's a static string.
 * @param htmlString The HTML template string.
 */
export function Template(htmlString: string) {
	return function (constructor: Function) { // Targets the class constructor
		const originalConnectedCallback = constructor.prototype.connectedCallback;
		constructor.prototype.connectedCallback = function(this: DualityComponent) { // Add 'this' type
			if (typeof this._setTemplate === 'function') {
				this._setTemplate(() => htmlString);
			}
			if (originalConnectedCallback) {
				originalConnectedCallback.call(this);
			}
		}
	};
}

// --- Example Usage: A Simple <hello-world> Component ---
// File: duality/examples/poc02-hello-component/hello-world.component.ts (Conceptual)

// For PoC, ensure DualityComponent and decorators are defined or imported before use.
// This example assumes they are in scope.

@CustomElement('hello-world') // Defines the tag name <hello-world>
@Template('<h1>Hello from Duality!</h1><p>This is my first Duality Web Component.</p><slot></slot>')
export class HelloWorldComponent extends DualityComponent {
	constructor() {
		super();
		console.log('HelloWorldComponent constructor called');
	}

	onConnected() {
		console.log('HelloWorldComponent connected to DOM!');
		const heading = this._shadowRoot.querySelector('h1');
		if (heading) {
			heading.style.color = 'purple';
		}
	}

	onDisconnected() {
		console.log('HelloWorldComponent disconnected from DOM.');
	}
}
