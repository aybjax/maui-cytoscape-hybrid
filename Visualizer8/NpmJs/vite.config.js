import { defineConfig } from "vite";

export default defineConfig({
    build: {
        outDir: '../wwwroot/js',
        minify: true,
        manifest: false,
        rollupOptions: {
            treeshake: true,
            input: './src/index.ts',
            output: {
                entryFileNames: `app.js`,
            },
        }
    }
})