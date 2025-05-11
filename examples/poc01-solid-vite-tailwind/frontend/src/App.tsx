import { onMount, createSignal, type Component } from 'solid-js';

const App: Component = () => {
	const [count, setCount] = createSignal(0);
	const [message, setMessage] = createSignal("Connecting to Duality backend...");

	onMount(async () => {
		await new Promise(res => setTimeout(res, 500));
		setMessage("SolidJS App Loaded! (Served by Vite, initial HTML by Deno/Hono)");
	});

	return (
		<div class="min-h-screen bg-gradient-to-br from-slate-900 to-purple-900 text-gray-100 flex flex-col items-center justify-center p-4 selection:bg-purple-500 selection:text-white">
			<div class="bg-slate-800 bg-opacity-80 backdrop-blur-lg shadow-2xl rounded-xl p-8 md:p-12 w-full max-w-2xl text-center">
				<header class="mb-8">
					<h1 class="text-5xl md:text-6xl font-bold text-transparent bg-clip-text bg-gradient-to-r from-purple-400 via-pink-500 to-red-500">
						Duality <span class="text-gray-300">&</span> SolidJS
					</h1>
					<p class="text-xl text-purple-300 mt-3">PoC 1: Basic Integration</p>
				</header>
				<div class="mb-8">
					<p class="text-lg text-gray-300 transition-opacity duration-500"
					   classList={{ 'opacity-10': message().includes('Connecting'), 'opacity-100': !message().includes('Connecting') }}>
						{message()}
					</p>
				</div>
				<div class="my-6">
					<button
						type="button"
						onClick={() => setCount(count() + 1)}
						class="px-8 py-3 bg-purple-600 hover:bg-purple-700 active:bg-purple-800 rounded-lg text-white font-semibold text-lg shadow-lg transform hover:scale-105 active:scale-95 transition-all duration-150 ease-in-out focus:outline-none focus:ring-4 focus:ring-purple-500 focus:ring-opacity-50"
					>
						Clicks: {count()}
					</button>
				</div>
				<p class="text-sm text-gray-500 mt-10">
					This SolidJS component is served by Vite and styled with Tailwind CSS.
					<br />The initial HTML page is served by Deno/Hono.
				</p>
			</div>
			<footer class="absolute bottom-4 text-center text-gray-600 text-xs">
				Duality Framework - PoC 1 (SolidJS Track)
			</footer>
		</div>
	);
};
export default App;