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
	private _templateFn: (() => string) | null = null;
	private _styles: CSSStyleSheet[] = [];

	private constructor() {
		super();
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
			this._shadowRoot.innerHTML = this._templateFn();
		} else {
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
	_addStyle(style: CSSStyleSheet) {
		this._styles.push(style);
	}

	// TODO: onConnected?(): void;
	// TODO: onDisconnected?(): void;
	// TODO: onPropsChanged?(changedProps: Map<string, any>): void;
	// TODO: beforeUpdate?(): void;
	// TODO: afterUpdate?(): void;
}

