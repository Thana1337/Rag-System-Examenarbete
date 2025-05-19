/** @type {import('tailwindcss').Config} */
export default {
  darkMode: 'class',
  content: [
    "./index.html",
    "./src/**/*.{js,jsx,ts,tsx}"
  ],
  theme: {
    extend: {
      colors: {
        warmStart: '#FF7E5F',    // coral accent
        warmEnd:   '#FEB47B',    // peach accent
        glassBase: 'rgba(255, 255, 255, 0.12)',
        glassBorder: 'rgba(255, 255, 255, 0.2)'
      },
      backdropBlur: {
        xs: '2px',   // subtle front-layer blur
        md: '12px',  // deeper panel blur
        xl: '24px',  // heavy background blur
      },
      fontFamily: {
        sans: ['Inter', 'ui-sans-serif', 'system-ui', 'sans-serif'],
      }
    },
  },
  plugins: [],
}
