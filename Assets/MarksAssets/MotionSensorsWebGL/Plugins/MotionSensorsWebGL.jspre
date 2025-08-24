Module['MotionSensorsWebGL'] = Module['MotionSensorsWebGL'] || {};

Module['MotionSensorsWebGL'].fromEulerAux = Module['MotionSensorsWebGL'].fromEulerAux || {
	Vector3: function(x, y, z) {
	    this.x = x || 0;
	    this.y = y || 0;
	    this.z = z || 0;
    },
	Quaternion: function(x, y, z, w) {
	    this._x = x || 0;
	    this._y = y || 0;
	    this._z = z || 0;
	    this._w = w || 1;
    },
	Matrix4: function() {
		this._zero = new Vector3(0, 0, 0);
		this._one = new Vector3(1, 1, 1);
		this.elements = [
			1, 0, 0, 0,
			0, 1, 0, 0,
			0, 0, 1, 0,
			0, 0, 0, 1
		];
	},
	Euler: function (x, y, z) {
		this._matrix = new Matrix4();
		this._x = x || 0;
		this._y = y || 0;
		this._z = z || 0;
	}
};

Object.assign(Module['MotionSensorsWebGL'].fromEulerAux.Matrix4.prototype, {
	compose: function ( position, quaternion, scale ) {
		const te = this.elements;
		const x = quaternion._x, y = quaternion._y, z = quaternion._z, w = quaternion._w;
		const x2 = x + x,	y2 = y + y, z2 = z + z;
		const xx = x * x2, xy = x * y2, xz = x * z2;
		const yy = y * y2, yz = y * z2, zz = z * z2;
		const wx = w * x2, wy = w * y2, wz = w * z2;

		const sx = scale.x, sy = scale.y, sz = scale.z;

		te[0] = (1 - (yy + zz)) * sx;
		te[1] = (xy + wz) * sx;
		te[2] = (xz - wy) * sx;
		te[3] = 0;

		te[4] = (xy - wz) * sy;
		te[5] = (1 - (xx + zz)) * sy;
		te[6] = (yz + wx) * sy;
		te[7] = 0;

		te[8] = (xz + wy) * sz;
		te[9] = (yz - wx) * sz;
		te[10] = (1 - (xx + yy)) * sz;
		te[11] = 0;

		te[12] = position.x;
		te[13] = position.y;
		te[14] = position.z;
		te[15] = 1;

		return this;

	},
	makeRotationFromQuaternion: function (q) {
		return this.compose(this._zero, q, this._one);
	},
});
	
	
Object.assign(Module['MotionSensorsWebGL'].fromEulerAux.Euler.prototype, {
	setFromRotationMatrix: function (m) {
		const clamp = function (value, min, max) {
			return Math.max( min, Math.min( max, value ) );
		}
		const te = m.elements;
		const m11 = te[0], m12 = te[4], m13 = te[8];
		const m21 = te[1], m22 = te[5], m23 = te[9];
		const m31 = te[2], m32 = te[6], m33 = te[10];

		this._x = Math.asin(-clamp(m23,-1,1));

		if (Math.abs(m23) < 0.9999999) {
			this._y = Math.atan2(m13, m33);
			this._z = Math.atan2(m21, m22);
		} else {
			this._y = Math.atan2(-m31, m11);
			this._z = 0;
		}

		return this;

	},
	setFromQuaternion: function (q) {
		this._matrix.makeRotationFromQuaternion(q);
		return this.setFromRotationMatrix(this._matrix);
	}
});

Object.assign(Module['MotionSensorsWebGL'].fromEulerAux.Quaternion.prototype, {
	setFromEuler: function (euler) {
		const x = euler._x, y = euler._y, z = euler._z;
		const c1 = Math.cos(x * 0.5);
		const c2 = Math.cos(y * 0.5);
		const c3 = Math.cos(z * 0.5);

		const s1 = Math.sin(x * 0.5);
		const s2 = Math.sin(y * 0.5);
		const s3 = Math.sin(z * 0.5);

		this._x = s1 * c2 * c3 + c1 * s2 * s3;
		this._y = c1 * s2 * c3 - s1 * c2 * s3;
		this._z = c1 * c2 * s3 - s1 * s2 * c3;
		this._w = c1 * c2 * c3 + s1 * s2 * s3;

		return this;

	},

	multiply: function (q) {
		return this.multiplyQuaternions(this, q);
	},

	multiplyQuaternions: function (a, b) {
		const qax = a._x, qay = a._y, qaz = a._z, qaw = a._w;
		const qbx = b._x, qby = b._y, qbz = b._z, qbw = b._w;

		this._x = qax * qbw + qaw * qbx + qay * qbz - qaz * qby;
		this._y = qay * qbw + qaw * qby + qaz * qbx - qax * qbz;
		this._z = qaz * qbw + qaw * qbz + qax * qby - qay * qbx;
		this._w = qaw * qbw - qax * qbx - qay * qby - qaz * qbz;

		return this;
	}
});


Module['MotionSensorsWebGL'].fromEuler = function (alpha, beta, gamma) {
    Vector3 = Module['MotionSensorsWebGL'].fromEulerAux.Vector3;
	Quaternion = Module['MotionSensorsWebGL'].fromEulerAux.Quaternion;
	Matrix4 = Module['MotionSensorsWebGL'].fromEulerAux.Matrix4;
	Euler = Module['MotionSensorsWebGL'].fromEulerAux.Euler;
	
	var quaternion = new Quaternion();
	var absoluteEuler = new Euler();
	var deltaEuler = new Euler();
	this.deltaY = this.deltaY || 0;
    this.previousYaw = this.previousYaw || 0;
	
	const degToRad = function (degrees) {return degrees * Math.PI / 180;};
	const orient = window.orientation ? degToRad(window.orientation) : 0;
    const halfAngle = -orient * 0.5;
    const s = Math.sin(halfAngle);
    const q0 = new Quaternion(0, 0, s, Math.cos(halfAngle));
    const q1 = new Quaternion(-Math.sqrt(0.5), 0, 0, Math.sqrt(0.5));

    const euler = new Euler(degToRad(beta), degToRad(alpha), degToRad(-gamma));

    quaternion.setFromEuler(euler);
    quaternion.multiply(q1);
    quaternion.multiply(q0);
	
	absoluteEuler.setFromQuaternion(quaternion);
	deltaEuler.setFromQuaternion(quaternion);
	
	if (!this.previousYaw) {
		this.previousYaw = absoluteEuler._y;
	} else {
		this.deltaY += absoluteEuler._y - this.previousYaw;
        deltaEuler._y = this.deltaY;
        this.previousYaw = absoluteEuler._y;
	}
	
	quaternion.setFromEuler(deltaEuler);

    return [quaternion._x, quaternion._y, quaternion._z, quaternion._w];
};

Module['MotionSensorsWebGL'].requestPermission = function() {
	if (typeof DeviceOrientationEvent !== 'undefined') {
		if (typeof DeviceOrientationEvent.requestPermission === 'function') {
			return DeviceOrientationEvent.requestPermission().then(
				function(permissionState) {
					if (permissionState === 'granted') {
						return new Promise(function(resolve, reject){
							window.addEventListener("devicemotion", function(event){
								if(event.rotationRate.alpha || event.rotationRate.beta || event.rotationRate.gamma)
									resolve(Object.create({
										get name() {
											return "PermissionState"
										},
										get message() {
											return "Granted"
										}
									}));
								else {
									reject(Object.create({
										get name() {
											return "DeviceNotSendingDataError"
										},
										get message() {
											return "Data not being sent. Gyroscope/Accelerometer might not be present, malfunctioning, or disabled. Is this a desktop?"
										}
									}));
								}
							}, { once: true });
						});
					} else {
						return Promise.reject(Object.create({
								get name() {
									return "PermissionState"
								},
								get message() {
									return "Denied"
								}
							})
						);
					}
				}
			).catch(
				function(err) {
					return Promise.reject(err);
				}
			)
		}
		else {
			return new Promise(function(resolve, reject){
				window.addEventListener("devicemotion", function(event){
					if(event.rotationRate.alpha || event.rotationRate.beta || event.rotationRate.gamma)
						resolve(Object.create({
							get name() {
								return "PermissionState"
							},
							get message() {
								return "Granted"
							}
						}));
					else {
						reject(Object.create({
							get name() {
								return "DeviceNotSendingDataError"
							},
							get message() {
								return "Data not being sent. Gyroscope/Accelerometer might not be present, malfunctioning, or disabled. Is this a desktop?"
							}
						}));
					}
				}, { once: true });
			});
		}
	} else {
		return new Promise(function (resolve,reject) {
			reject(
				Object.create({
					get name() {
						return "UnsupportedAPIError"
					},
					get message() {
						return "Your browser does not support the \'DeviceOrientationEvent\' and/or \'DeviceMotionEvent\' APIs"
					}
				})
			)
		});
	}
};

Module['MotionSensorsWebGL'].requestPermissionClbks = function(result) {
	this.requestPermissionClbksInternal = this.requestPermissionClbksInternal || Module.cwrap("MotionSensorsWebGL_requestPermissionClbks", null, ["string"]);
	this.requestPermissionClbksInternal(result);
};


Module['MotionSensorsWebGL'].devicemotion = function(event) {
	if (Module['MotionSensorsWebGL'].devicemotionArr.byteLength === 0)//buffer resized, need to assign array again
		Module['MotionSensorsWebGL'].devicemotionArr = new Float32Array(buffer, Module['MotionSensorsWebGL'].devicemotionRef, 10);
	Module['MotionSensorsWebGL'].devicemotionArr[0] = event.acceleration.x;
	Module['MotionSensorsWebGL'].devicemotionArr[1] = event.acceleration.y;
	Module['MotionSensorsWebGL'].devicemotionArr[2] = event.acceleration.z;
	Module['MotionSensorsWebGL'].devicemotionArr[3] = event.accelerationIncludingGravity.x;
	Module['MotionSensorsWebGL'].devicemotionArr[4] = event.accelerationIncludingGravity.y;
	Module['MotionSensorsWebGL'].devicemotionArr[5] = event.accelerationIncludingGravity.z;
	Module['MotionSensorsWebGL'].devicemotionArr[6] = event.rotationRate.alpha;
	Module['MotionSensorsWebGL'].devicemotionArr[7] = event.rotationRate.beta;
	Module['MotionSensorsWebGL'].devicemotionArr[8] = event.rotationRate.gamma;
	Module['MotionSensorsWebGL'].devicemotionArr[9] = event.interval;
};

Module['MotionSensorsWebGL'].deviceorientation = function(event) {
	if (Module['MotionSensorsWebGL'].deviceorientationArr.byteLength === 0) {//buffer resized, need to assign array again
		Module['MotionSensorsWebGL'].deviceorientationArr = new Float32Array(buffer, Module['MotionSensorsWebGL'].deviceorientationRef, 9);
		Module['MotionSensorsWebGL'].deviceorientationLast= new Uint8Array(buffer, Module['MotionSensorsWebGL'].deviceorientationRef + 36, 1);
	}

	var quaternion = Module['MotionSensorsWebGL'].fromEuler(event.alpha, event.beta, event.gamma);
	
	Module['MotionSensorsWebGL'].deviceorientationArr[0] = event.alpha;
	Module['MotionSensorsWebGL'].deviceorientationArr[1] = event.beta;
	Module['MotionSensorsWebGL'].deviceorientationArr[2] = event.gamma;
	Module['MotionSensorsWebGL'].deviceorientationArr[3] = event.webkitCompassHeading;
	Module['MotionSensorsWebGL'].deviceorientationArr[4] = event.webkitCompassAccuracy;
	Module['MotionSensorsWebGL'].deviceorientationArr[5] = -quaternion[0];
	Module['MotionSensorsWebGL'].deviceorientationArr[6] = -quaternion[1];
	Module['MotionSensorsWebGL'].deviceorientationArr[7] = quaternion[2];
	Module['MotionSensorsWebGL'].deviceorientationArr[8] = quaternion[3];
	Module['MotionSensorsWebGL'].deviceorientationLast[0]= event.absolute;
};

