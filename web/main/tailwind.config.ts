/** @type {import('tailwindcss').Config} */

export default {
	content: ['./src/**/*.{html,js,svelte,ts}'],
	theme: {
		extend: {
			colors: {
				'global-dark-blue': '#1e1e2e'
			},
			fontFamily: {
				orbitron: ['Orbitron', 'sans-serif'],
				roboto: ['Roboto', 'sans-serif']
			}
		}
	},
	plugins: []
};
