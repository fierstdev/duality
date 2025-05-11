import { defineConfig } from 'vite'; // Import from npm specifier
import solidPlugin from 'vite-plugin-solid'; // Import from npm specifier
import tailwindcss from '@tailwindcss/vite';

export default defineConfig({
	plugins: [
		solidPlugin(),
		tailwindcss()
	],
	build: {
		target: 'esnext',
	},
	server: {
		port: 5173,
	}
});