#version 300 es
precision highp float;

uniform float time;
uniform vec2 resolution;

in vec2 vUv;
out vec4 outColor;

const float PI = acos(-1.0);

#include './module/chars.glsl'

float sprite(uvec4 ch, vec2 uv, ivec2 offset) {
  const vec2 size = vec2(8, 12);
  ivec2 idx = ivec2(floor(vec2(uv.x, 1.0 - uv.y) * size));

  ivec2 pos = idx + offset;
  uint bit = ch[int(pos.y / 3)];
  uint shifted = 1u & (bit << (8 * (pos.y % 3) + pos.x) >> 23);

  bool bounds = all(lessThan(pos, ivec2(size))) && all(greaterThanEqual(pos, ivec2(0)));
  return float(shifted) * float(bounds);
}

void main() {
  vec2 uv = vUv, suv = vUv * 2.0 - 1.0;
  vec2 auv = uv * resolution / min(resolution.x, resolution.y);
  float scale = 5.0;
  vec2 map = auv * vec2(12.0 / 8.0, 1.0) * scale;

  uvec4[10] chars = uvec4[](
    ch_W, ch_e, ch_b, ch_G, ch_L,
    ch_spc,
    ch_1, ch_0, ch_crs, ch_3
  );
  int len = chars.length();

  float result;
  for(int i = 0; i < len; i++) {
    int y = int(floor((sin(-time * 2.0 + float(i) / float(len) * PI * 2.0) * 0.5 + 0.5) * 12.0));
    // int y = 0;
    result += sprite(chars[i], map, ivec2(- (8 * i + 2), y));
  }

  outColor = vec4(vec3(result), 1.0);

  vec2 px = 1.0 / resolution;
  px *= resolution / min(resolution.x, resolution.y);
  px *= vec2(12.0 / 8.0, 1.0) * scale;

  float line;
  line += step(fract(map.x *  8.0 + px.x * scale), fract(px.x *  8.0));
  line += step(fract(map.y * 12.0 + px.y * scale), fract(px.y * 12.0));
  line = min(1.0, line) * 0.08;
  outColor += line;

  float s = 0.1;
  line  = step(fract(map.x *  8.0 * s + px.x * scale * s), fract(px.x *  8.0 * s));
  line += step(fract(map.y * 12.0 * s + px.y * scale * s), fract(px.y * 12.0 * s));
  line = min(1.0, line) * 0.2;
  outColor += line;

  // memo
  // 0xC6 = 0b 1100 0110
  // uint bit = uint(0x007CC6); // 16 = 2^4 -> 4 * 6 = 24bits
  // 0x6 = 0b0110
  // bit = 1u & (bit << (4 * 5 + 0)) >> 23; // 0
}