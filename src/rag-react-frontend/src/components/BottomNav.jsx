// src/components/BottomNav.jsx
import React, { useEffect } from 'react';
import { NavLink, useNavigate } from 'react-router-dom';
import { motion, useAnimation } from 'framer-motion';

const NAV_ITEMS = [
  { name: 'Home',       to: '/'      },
  { name: 'Upload PDF', to: '/upload'},
  { name: 'Ask me',     to: '/chat'  },
];

const curtainVariants = {
  initial: { clipPath: 'inset(0 0 100% 0)' },
  animate: {
    clipPath: ['inset(0 0 100% 0)', 'inset(0 0 50% 0)', 'inset(0 0 0% 0)'],
    transition: { duration: 0.5, ease: 'easeInOut', times: [0, 0.5, 1] }
  },
  exit: {
    clipPath: ['inset(0 0 0% 0)', 'inset(0 0 50% 0)', 'inset(0 0 100% 0)'],
    transition: { duration: 0.5, ease: 'easeInOut', times: [0, 0.4, 1] }
  }
};

// Module‐scope flag so we only delay once
let navPlayed = false;

export default function BottomNav() {
  const controls = useAnimation();
  const navigate = useNavigate();

  // Only the very first mount should animate in with delay
  const shouldAnimate = !navPlayed;
  useEffect(() => {
    if (shouldAnimate) {
      navPlayed = true;
    }
  }, [shouldAnimate]);

  const handleClick = async (e, to) => {
    e.preventDefault();
    controls.set('initial');
    await controls.start('animate');
    navigate(to);
    await controls.start('exit');
  };

  return (
    <>
      <motion.div
        className="fixed inset-0 z-50 curtain"
        variants={curtainVariants}
        initial="initial"
        animate={controls}
      />

      <motion.nav
        initial={shouldAnimate ? { y: '-100vh', opacity: 0 } : undefined}
        animate={shouldAnimate ? { y: 0,     opacity: 1 } : undefined}
        transition={
          shouldAnimate
            ? { duration: 0.5, type: 'spring', stiffness: 50, damping: 25, delay: 4.5 }
            : {}
        }
        className="fixed bottom-4 left-1/2 transform -translate-x-1/2 z-40 overflow-hidden"
      >
        <div className="bg-black px-3 py-3 rounded-full flex items-center">
          {NAV_ITEMS.map(item => (
            <NavLink
              key={item.to}
              to={item.to}
              end
              onClick={e => handleClick(e, item.to)}
              className={({ isActive }) =>
                `relative px-4 py-2 text-sm font-medium transition-colors cursor-none ${
                  isActive ? 'text-black' : 'text-white'
                }`
              }
            >
              {({ isActive }) => (
                <>
                  {isActive && (
                    <motion.div
                      layoutId="nav-pill"
                      className="absolute inset-0 bg-white rounded-full"
                      transition={{ type: 'spring', stiffness: 300, damping: 30 }}
                    />
                  )}
                  <span className="relative">{item.name}</span>
                </>
              )}
            </NavLink>
          ))}
        </div>
      </motion.nav>
    </>
  );
}
