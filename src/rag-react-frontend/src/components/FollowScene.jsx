// src/components/FollowScene.jsx
import React from 'react';
import Spline from '@splinetool/react-spline';

export default function FollowScene() {
  return (
    <div className="inset-20 absolute overflow-hidden">
      <Spline
        scene="https://prod.spline.design/6wuwS7c9CagocvnV/scene.splinecode"
        className="absolute inset-0 w-full h-full"
        style={{ pointerEvents: 'all' }}
      />
    </div>
  );
}
