import React, { useEffect } from 'react';
import { motion, useMotionValue, useTransform } from 'framer-motion';

export default function MagneticGrid({
  lineCount = 20,
  threshold = 150,
  maxOffset = 60,
  color = '#313131',
}) {
  const mouseX = useMotionValue(-9999);
  const mouseY = useMotionValue(-9999);

  useEffect(() => {
    const move = ({ clientX, clientY }) => {
      mouseX.set(clientX);
      mouseY.set(clientY);
    };
    window.addEventListener('mousemove', move);
    return () => window.removeEventListener('mousemove', move);
  }, [mouseX, mouseY]);

  const vs = Array.from({ length: lineCount }, (_, i) => {
    const x = ((i + 1) * 100) / (lineCount + 1) + '%';
    const offsetX = useTransform(
      mouseX,
      [0, window.innerWidth],
      [maxOffset, -maxOffset]
    );
    return (
      <motion.div
        key={`v-${i}`}
        style={{
          position: 'absolute',
          top: 0,
          left: x,
          width: 1,
          height: '100%',
          background: color,
          x: offsetX,
        }}
        transition={{ type: 'spring', stiffness: 200, damping: 30 }}
      />
    );
  });

  const hs = Array.from({ length: lineCount }, (_, i) => {
    const y = ((i + 1) * 100) / (lineCount + 1) + '%';
    const offsetY = useTransform(
      mouseY,
      [0, window.innerHeight],
      [maxOffset, -maxOffset]
    );
    return (
      <motion.div
        key={`h-${i}`}
        style={{
          position: 'absolute',
          left: 0,
          top: y,
          height: 1,
          width: '100%',
          background: color,
          y: offsetY,
        }}
        transition={{ type: 'spring', stiffness: 200, damping: 30 }}
      />
    );
  });

  return (
    <div className="pointer-events-none fixed inset-0 overflow-hidden z-0">
      {vs}
      {hs}
    </div>
  );
}
