import * as THREE from 'three'
import { RawShaderMaterial } from './ExtendedMaterials'
import vertexShader from './shader/plane.vs'
import fragmentShader from './shader/plane.fs'
import { pane } from '../Gui'

export class Canvas {
  private readonly renderer: THREE.WebGLRenderer
  private readonly scene: THREE.Scene
  private readonly camera: THREE.PerspectiveCamera
  private readonly clock: THREE.Clock
  private readonly material: RawShaderMaterial

  constructor(private readonly canvas: HTMLCanvasElement) {
    this.renderer = this.createRenderer()
    this.scene = new THREE.Scene()
    this.camera = this.createCamera()
    this.clock = new THREE.Clock()
    this.material = this.createPlane()

    this.renderer.setAnimationLoop(this.render.bind(this))
    window.addEventListener('resize', this.resize.bind(this))
  }

  private createRenderer() {
    const renderer = new THREE.WebGLRenderer({ canvas: this.canvas })
    renderer.setSize(window.innerWidth, window.innerHeight)
    renderer.setPixelRatio(window.devicePixelRatio)
    return renderer
  }

  private createCamera() {
    const camera = new THREE.PerspectiveCamera()
    camera.matrixAutoUpdate = false
    return camera
  }

  private createPlane() {
    const geo = new THREE.PlaneGeometry(2, 2)
    const mat = new RawShaderMaterial({
      uniforms: {
        time: { value: 0 },
        resolution: { value: [window.innerWidth * this.renderer.getPixelRatio(), window.innerHeight * this.renderer.getPixelRatio()] },
      },
      vertexShader,
      fragmentShader,
    })
    this.scene.add(new THREE.Mesh(geo, mat))
    return mat
  }

  private render() {
    pane.updateFps()

    const dt = this.clock.getDelta()
    this.material.uniforms.time.value += dt

    this.renderer.setRenderTarget(null)
    this.renderer.render(this.scene, this.camera)
  }

  private resize() {
    const width = window.innerWidth
    const height = window.innerHeight

    this.renderer.setSize(width, height)
    this.material.uniforms.resolution.value = [width * this.renderer.getPixelRatio(), height * this.renderer.getPixelRatio()]
  }

  dispose() {
    this.renderer.setAnimationLoop(null)
    this.renderer.dispose()
  }
}
