//
//  RaygunDefines.h
//  NetworkMonitor
//
//  Created by Mitchell Duncan on 10/06/19.
//

#ifndef RaygunDefines_h
#define RaygunDefines_h

#import <Foundation/Foundation.h>

#if TARGET_OS_IOS || TARGET_OS_TV
#define RAYGUN_CAN_USE_UIDEVICE 1
#else
#define RAYGUN_CAN_USE_UIDEVICE 0
#endif

#if RAYGUN_CAN_USE_UIDEVICE
#define RAYGUN_CAN_USE_UIKIT 1
#else
#define RAYGUN_CAN_USE_UIKIT 0
#endif

static char const * const kSessionTaskIdKey = "RaygunSessionTaskId";

#endif /* RaygunDefines_h */
