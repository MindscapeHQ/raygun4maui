//
//  RaygunNetworkMonitor.h
//  RaygunNetworkMonitor
//
//  Created by Mitchell Duncan on 10/06/19.
//

#ifndef RaygunNetworkMonitor_h
#define RaygunNetworkMonitor_h

#import <Foundation/Foundation.h>

@interface RaygunNetworkMonitor : NSObject

@property (nonatomic, class, readonly, copy) NSString *RequestOccurredNotificationName;
@property (nonatomic, class, readonly, copy) NSString *RequestUrlNotificationKey;
@property (nonatomic, class, readonly, copy) NSString *RequestMethodNotificationKey;
@property (nonatomic, class, readonly, copy) NSString *RequestDurationNotificationKey;

+ (void)enable;
+ (void)networkRequestStarted:(NSURLSessionTask *)task;
+ (void)networkRequestEnded:(NSURLRequest *)request withTaskId:(NSString *)taskId;
+ (void)networkRequestCanceled:(NSString *)taskId;

@end

#endif /* RaygunNetworkMonitor_h */
