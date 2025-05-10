/**
* @CustomElement decorator
* Registers the component class as a custom element with the given tag name.
* @param tagName The tag name for the custom element (e.g., 'my-element').
*/
export function CustomElement(tagName: string) {
	return function (constructor: CustomElementConstructor) {
		if (customElements.get(tagName)) {
			console.warn(`Custom element with tag name '${tagName}' is already defined. Skipping redefinition.`);
		} else {
			customElements.define(tagName, constructor);
		}
	};
}

/**
 * @Template decorator
 * Associates an HTML template string with a DualityComponent.
 * The template string can later support bindings.
 * For this PoC, it's a static string.
 * @param htmlString The HTML template string.
 */
export function Template(htmlString: string) {
	return function (constructor: Function) {
		const originalConnectedCallback = constructor.prototype.connectedCallback;
		constructor.prototype.connectedCallback = function (this: DualityComponent) {

		}
	};
}