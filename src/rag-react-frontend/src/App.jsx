// src/App.jsx
import React, { useEffect, useState } from 'react';
import {
  BrowserRouter,
  Routes,
  Route,
  useLocation
} from 'react-router-dom';
import { AnimatePresence, motion, useAnimation } from 'framer-motion';

import CustomCursor             from './components/CustomCursor';
import Header                   from './components/header/Header';
import BottomNav                from './components/BottomNav';
import Hero                     from './components/Hero';
import ChatView                 from './components/chatview/ChatView';
import About                    from './components/about/About';
import UploadPage               from './components/upload/UploadPage';
import MagneticGrid             from './components/MagneticGrid';
import ParticleCircleBackground from './components/ParticleCircleBackground';

// Page transition variants
const pageVariants   = {
  initial: { opacity: 0, y: 20 },
  animate: { opacity: 1, y: 0 },
  exit:    { opacity: 0, y: -20 }
};
const pageTransition = { duration: 0.1, ease: 'easeInOut' };

// Only ever scramble once
let hasIntroPlayed = false;

function Main() {
  const location    = useLocation();
  const particleAnim = useAnimation();
  const [ready, setReady] = useState(false);

  // Determine if we should do the scramble intro this very first time
  const isRoot         = location.pathname === '/';
  const shouldScramble = isRoot && !hasIntroPlayed;

  useEffect(() => {
    if (shouldScramble) {
      // Mark that we've played it, so we never do it again
      hasIntroPlayed = true;
      // Prepare animation: start “zoomed in”
      particleAnim.set({ scale: 2 });

      // Then animate to normal over 3s, then reveal the UI
      particleAnim
        .start({ scale: 1, transition: { duration: 1, ease: 'easeOut' } })
        .then(() => setReady(true));
    } else {
      // Not first load or not on root → show immediately
      setReady(true);
    }
  }, []); // <-- run ONCE on mount

  return (
    <>
    <motion.div
        className="pointer-events-none fixed inset-0 z-0"
        
      >
      <div

        >
      <ParticleCircleBackground
        particleCount={2000}
        circleRadiusRatio={0.5}
        repelRadius={100}
        repelStrength={0.7}
        particleSize={0.5}
        
        onScrambleComplete={() => {
              hasIntroPlayed = true;
              setReady(true);
            }}
      />

      </div>
    </motion.div>

      

      {/* Delay all UI until scramble is done */}
      {ready && (
        <div className="relative cursor-none z-10">
          <CustomCursor />
          <Header />
          <BottomNav />
          <AnimatePresence mode="wait">
            <Routes location={location} key={location.pathname}>
              <Route
                path="/"
                element={
                  <motion.div
                    variants={pageVariants}
                    initial="initial"
                    animate="animate"
                    exit="exit"
                    transition={pageTransition}
                    className="h-screen"
                  >
                    <Hero />
                  </motion.div>
                }
              />
              <Route
                path="/chat"
                element={
                  <motion.div
                    variants={pageVariants}
                    initial="initial"
                    animate="animate"
                    exit="exit"
                    transition={pageTransition}
                  >
                    <ChatView />
                  </motion.div>
                }
              />
              <Route
                path="/about"
                element={
                  <motion.div
                    variants={pageVariants}
                    initial="initial"
                    animate="animate"
                    exit="exit"
                    transition={pageTransition}
                  >
                    <About />
                  </motion.div>
                }
              />
              <Route
                path="/upload"
                element={
                  <motion.div
                    variants={pageVariants}
                    initial="initial"
                    animate="animate"
                    exit="exit"
                    transition={pageTransition}
                  >
                    <UploadPage />
                  </motion.div>
                }
              />
            </Routes>
          </AnimatePresence>
        </div>
      )}
    </>
  );
}

export default function App() {
  return (
    <BrowserRouter>
      <Main />
    </BrowserRouter>
  );
}
