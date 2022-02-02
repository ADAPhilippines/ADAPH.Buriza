const path = require("path");
const webpack = require('webpack');

module.exports = {
    entry: './wwwroot/scripts/App.ts',
    module: {
        rules: [
            {
                test: /\.tsx?$/,
                use: 'ts-loader',
                exclude: /node_modules/,
            },
        ],
    },
    resolve: {
        extensions: ['.tsx', '.ts', '.js'],
        fallback:  { "stream": require.resolve("stream-browserify") }
    },
    experiments: {
        asyncWebAssembly: true
    },
    plugins: [
        new webpack.ProvidePlugin({
            Buffer: ['buffer', 'Buffer'],
        }),
    ],
    output: {
        path: path.resolve(__dirname, './wwwroot/dist/script'),
        filename: "app.bundle.js"
    }
};