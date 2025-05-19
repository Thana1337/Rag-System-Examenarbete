// src/components/CustomCursor.jsx
import React, { useEffect } from 'react';
import { motion, useMotionValue, useSpring } from 'framer-motion';

export default function CustomCursor() {
  // raw pointer positions
  const x = useMotionValue(-100);
  const y = useMotionValue(-100);

  // stiffer, less bouncy spring
  const springConfig = { stiffness: 1500, damping: 100, mass: 0.3 };
  const springX = useSpring(x, springConfig);
  const springY = useSpring(y, springConfig);

  useEffect(() => {
    const move = e => {
      x.set(e.clientX);
      y.set(e.clientY);
    };
    window.addEventListener('pointermove', move);
    return () => window.removeEventListener('pointermove', move);
  }, [x, y]);

  return (
    <motion.div
      style={{
        translateX: springX,
        translateY: springY
      }}
      className="
        pointer-events-none fixed 
        w-6 h-6 bg-white/70 rounded-full 
        -translate-x-1/2 -translate-y-1/2 
        z-50
      "
    />
  );
}
