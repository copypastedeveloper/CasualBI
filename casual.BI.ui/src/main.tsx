import React from 'react'
import ReactDOM from 'react-dom'
import App from './App.tsx'
import 'webdatarocks/webdatarocks.css'
import './index.css'
import '@fontsource/public-sans';
import 'bootstrap/dist/css/bootstrap-reboot.min.css'
import 'bootstrap/dist/css/bootstrap.min.css'

//@ts-ignore
BigInt.prototype.toJSON = function (): string {
  return this.toString();
};

ReactDOM.render(
  <React.StrictMode>
      <App />
  </React.StrictMode>,
  document.getElementById('root')
)
