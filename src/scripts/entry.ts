import { Canvas } from './webgl/Canvas'

const canvas = new Canvas(document.querySelector<HTMLCanvasElement>('canvas')!)

window.addEventListener('beforeunload', () => {
	canvas.dispose()
})
