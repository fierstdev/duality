/* @refresh reload */
import { render } from 'solid-js/web'; // Import from npm specifier
import './index.css';
import App from "./App.tsx";

const root = document.getElementById('root');

if (!root) {
	throw new Error('Root element not found in HTML.');
}

render(() => <App />, root!);