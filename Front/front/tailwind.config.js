/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        primary: '#000B27',
        secondary: '#031740',
        accent: '#556475',
        blue: '#2569A5',
        lightblue: '#BACCD9',
      },
    },
  },
  plugins: [],
}
