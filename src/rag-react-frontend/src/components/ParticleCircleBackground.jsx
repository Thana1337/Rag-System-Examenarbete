import React, { useRef, useEffect } from 'react';

export default function ParticleCircleBackground({
  particleCount     = 2000,
  circleRadiusRatio = 0.4,
  maxSpeed          = 0.1,
  wanderStrength    = 0.02,
  repelRadius       = 100,
  repelStrength     = 0.4,
  particleColor     = 'rgba(255,255,255,0.8)',
  particleSize      = 1.2,
  scrambleDuration  = 5000,              // ms
  onScrambleComplete = () => {}
}) {
  const canvasRef = useRef(null);
  const mouse     = useRef({ x: -9999, y: -9999 });
  const particles = useRef([]);
  const startTime = useRef(performance.now());
  const doneRef   = useRef(false);

  useEffect(() => {
    const canvas = canvasRef.current;
    const ctx    = canvas.getContext('2d');
    let width  = (canvas.width  = window.innerWidth);
    let height = (canvas.height = window.innerHeight);

    const cx   = width / 2;
    const cy   = height / 2;
    const maxR = Math.min(width, height) * circleRadiusRatio;

    // 1) create particles with random start + circular target
    particles.current = Array.from({ length: particleCount }).map(() => {
      const angle = Math.random() * Math.PI * 2;
      const r     = Math.sqrt(Math.random()) * maxR;
      return {
        initX:   Math.random() * width,
        initY:   Math.random() * height,
        targetX: cx + r * Math.cos(angle),
        targetY: cy + r * Math.sin(angle),
        x: 0, y: 0,
        vx: 0, vy: 0
      };
    });

    // mouse tracking
    const onMove = e => {
      mouse.current.x = e.clientX;
      mouse.current.y = e.clientY;
    };
    window.addEventListener('mousemove', onMove);

    // resize handling
    const onResize = () => {
      width  = canvas.width  = window.innerWidth;
      height = canvas.height = window.innerHeight;
    };
    window.addEventListener('resize', onResize);

    let frameId;
    const draw = () => {
      ctx.clearRect(0, 0, width, height);

      const now     = performance.now();
      const elapsed = now - startTime.current;
      const t       = Math.min(elapsed / scrambleDuration, 1);
      const ease    = 1 - Math.pow(1.5 - t, 5); // cubic ease-out

      particles.current.forEach(p => {
        if (t < 1) {
          // SCRAMBLE PHASE: smoothly lerp into circle
          p.x = p.initX + (p.targetX - p.initX) * ease;
          p.y = p.initY + (p.targetY - p.initY) * ease;
        } else {
          // ONCE after intro finishes:
          if (!doneRef.current) {
            // lock perfectly on target
            p.x = p.targetX;
            p.y = p.targetY;
            doneRef.current = true;
            onScrambleComplete();
          }
          // WANDER + REPEL
          p.vx += (Math.random() - 0.6) * wanderStrength;
          p.vy += (Math.random() - 0.6) * wanderStrength;
          const dx = p.x - mouse.current.x;
          const dy = p.y - mouse.current.y;
          const dist = Math.hypot(dx, dy);
          if (dist < repelRadius) {
            const force = ((repelRadius - dist) / repelRadius) * repelStrength;
            p.vx += (dx / dist) * force;
            p.vy += (dy / dist) * force;
          }
          p.x += p.vx;
          p.y += p.vy;
          p.vx *= 0.99;
          p.vy *= 0.99;
          // wrap edges
          if (p.x < 0) p.x += width; else if (p.x > width) p.x -= width;
          if (p.y < 0) p.y += height; else if (p.y > height) p.y -= height;
        }

        // DRAW PARTICLE
        ctx.beginPath();
        ctx.arc(p.x, p.y, particleSize, 0, Math.PI * 2);
        ctx.fillStyle = particleColor;
        ctx.fill();
      });

      frameId = requestAnimationFrame(draw);
    };

    draw();

    return () => {
      cancelAnimationFrame(frameId);
      window.removeEventListener('mousemove', onMove);
      window.removeEventListener('resize', onResize);
    };
  }, []);

  return (
    <canvas
      ref={canvasRef}
      className="pointer-events-none fixed inset-0 z-0"
    />
  );
}
