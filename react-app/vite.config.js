import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
    plugins: [react()],
    base: '/react/', // Đường dẫn trong thư mục wwwroot
    build: {
        outDir: '../wwwroot/react', // Đường dẫn output cho build (wwwroot/react)
        emptyOutDir: true, // Xóa thư mục trước khi build lại
    },
})

